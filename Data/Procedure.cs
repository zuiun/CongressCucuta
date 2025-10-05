using System.Diagnostics;
using congress_cucuta.Converters;

namespace congress_cucuta.Data;

internal abstract class Procedure (
    IDType id,
    Procedure.Effect[] effects,
    bool isActiveStart = true
): IID {
    /*
     * Presence indicates DeclaredProcedure
     *
     * Targets Value (populated), Currencies (null: declarer's)
     */
    internal readonly struct Confirmation (Confirmation.CostType cost, byte value = 0) {
        /*
         * Always: always succeeds
         * DivisionChamber: simple majority vote, succeeds if vote passes
         * CurrencyValue: succeeds if Currency is higher than Value, Value is subtracted from Currency
         * SingleDiceValue: rolls one dice, succeeds if dice >= Value
         * SingleDiceCurrency: rolls one dice, succeeds if dice <= Currency, dice is subtracted from Currency
         * AdversarialDice: rolls two die representing declarer and others, whichever dice is higher gets positive effects of Procedure
         */
        internal enum CostType {
            Always,
            DivisionChamber,
            CurrencyValue,
            SingleDiceValue,
            SingleDiceCurrency,
            // AdversarialDice,
        }

        public CostType Cost => cost;
        public byte Value => value;
    }

    internal readonly struct Effect (
        Effect.ActionType action,
        IDType[] targetIDs,
        byte value = 0,
        params IDType[] filterIDs
    ) {
        internal enum ActionType {
            /*
             * Adds Value votes for the passage of Filter Ballot
             *
             * Targeted
             * Filters Ballots (empty: every, populated: specified)
             */
        VotePassAdd,
            /*
             * Adds Value votes for the failure of Filter Ballot
             *
             * Targeted
             * Filters Ballots (empty: every, populated: specified)
             */
            VoteFailAdd,
            /*
             * Causes Filter Ballot to require a two-thirds majority to pass
             *
             * Targeted
             * Filters Ballots (empty: every, populated: specified)
             */
            VotePassTwoThirds,
            /*
             * Adds Value to Target Currency
             *
             * Targeted
             * Targets Factions (populated: specified), Currencies (empty: STATE)
             * Filters Ballots (empty: every, populated: specified)
             */
            CurrencyAdd,
            /*
             * Subtracts Value from Target Currency
             *
             * Targeted
             * Targets Factions (populated: specified), Currencies (empty: STATE)
             * Filters Ballots (empty: every, populated: specified)
             */
            CurrencySubtract,
            /*
             * Initialises Currencies
             * This is only used for display and is handled elsewhere 
             *
             * Targeted
             */
            CurrencyInitialise,
            /*
             * Activates Target ProcedureImmediate
             *
             * Targeted
             * Targets Procedure (populated [one]: specified)
             * Filters Ballots (empty: every, populated: specified)
             */
            ProcedureActivate,
            /*
             * Assigns Target Roles to Regions
             *
             * Immediate, Declared
             * Targets Roles (empty: every, populated: excluded)
             */
            ElectionRegion,
            /*
             * Assigns Target Roles to Parties
             *
             * Immediate, Declared
             * Targets Roles (empty: every, populated: excluded)
             */
            ElectionParty,
            /*
             * Elects Target Role
             *
             * Immediate, Declared
             * Targets Roles (populated [one]: specified)
             */
            ElectionNominated,
            /*
             * Appoints (randomly when Immediate) Target Role
             *
             * Immediate, Declared
             * Targets Roles (populated [one]: specified)
             */
            ElectionAppointed,
            /*
             * Prevents Target Roles from voting current Ballot
             *
             * Declared
             * Targets Roles (empty: declarer excluded, populated: specified excluded)
             */
            VotersLimit,
        }

        public ActionType Action => action;
        public byte Value => value;
        public IDType[] TargetIDs => targetIDs;
        public IDType[] FilterIDs => filterIDs;

        private static string TargetToString (Effect effect, ref readonly Localisation localisation) {
            switch (effect.Action) {
                case ActionType.CurrencyAdd:
                case ActionType.CurrencySubtract:
                    if (effect.TargetIDs.Length > 0) {
                        List<string> factions = [];
                        var partiesIter = localisation.Parties.Keys.Where (p => effect.TargetIDs.Contains (p.ID));
                        var regionsIter = localisation.Regions.Keys.Where (r => effect.TargetIDs.Contains (r.ID));

                        foreach (var p in partiesIter) {
                            factions.Add (localisation.GetFactionOrAbbreviation (p));
                        }

                        foreach (var r in regionsIter) {
                            factions.Add (localisation.GetFactionOrAbbreviation (r));
                        }

                        return $"{string.Join (", ", factions)}:";
                    } else {
                        return string.Empty;
                    }
                case ActionType.ProcedureActivate: {
                    return localisation.Procedures[effect.TargetIDs[0]].Item1;
                }
                case ActionType.ElectionRegion:
                case ActionType.ElectionParty: {
                    if (effect.TargetIDs.Length > 0) {
                        List<string> excluded = [];

                        foreach (IDType t in effect.TargetIDs) {
                            excluded.Add (localisation.Roles[t].Item2);
                        }

                        return $"Everyone except {string.Join (", ", excluded)}:";
                    } else {
                        return "Everyone:";
                    }
                }
                case ActionType.ElectionNominated:
                case ActionType.ElectionAppointed:
                    return localisation.Roles[effect.TargetIDs[0]].Item1;
                case ActionType.VotersLimit: {
                    if (effect.TargetIDs.Length > 0) {
                        List<string> excluded = [];

                        foreach (IDType t in effect.TargetIDs) {
                            excluded.Add (localisation.Roles[t].Item2);
                        }

                        return $"Everyone except {string.Join (", ", excluded)}:";
                    } else {
                        return "Everyone except Declarer:";
                    }
                }
                default:
                    throw new NotSupportedException ();
            };
        }

        private static string FilterToString (Effect effect, ref readonly Localisation localisation) {
            switch (effect.Action) {
                case ActionType.VotePassAdd:
                case ActionType.VoteFailAdd:
                case ActionType.VotePassTwoThirds:
                case ActionType.CurrencyAdd:
                case ActionType.CurrencySubtract:
                case ActionType.ProcedureActivate:
                    if (effect.FilterIDs.Length > 0) {
                        var ballotsIter = localisation.Ballots.Where (k => effect.FilterIDs.Contains (k.Key))
                            .Select (k => k.Value.Item1);

                        return $"{string.Join (", ", ballotsIter)}:";
                    } else {
                        return "Every Ballot:";
                    }
                default:
                    throw new NotSupportedException ();
            }
        }

        public string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
            List<string> result = [];

            switch (Action) {
                case ActionType.VotePassAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Gains {Value} vote(s) in favour", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.VoteFailAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Gains {Value} vote(s) in opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.VotePassTwoThirds: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Needs a two-thirds majority to pass", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.CurrencyAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);

                    if (TargetIDs.Length > 0) {
                        string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                        string action = StringLineFormatter.Indent ($"Gains {Value} {localisation.Currencies[TargetIDs[0]]}", 3);

                        result.Add (filter);
                        result.Add (target);
                        result.Add (action);
                    } else {
                        string action = StringLineFormatter.Indent ($"Gain {Value} {localisation.Currencies[Currency.STATE]}", 2);

                        result.Add (filter);
                        result.Add (action);
                    }

                    break;
                }
                case ActionType.CurrencySubtract: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    
                    if (TargetIDs.Length > 0) {
                        string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                        string action = StringLineFormatter.Indent ($"Loses {Value} {localisation.Currencies[TargetIDs[0]]}", 3);

                        result.Add (filter);
                        result.Add (target);
                        result.Add (action);
                    } else {
                        string action = StringLineFormatter.Indent ($"Lose {Value} {localisation.Currencies[Currency.STATE]}", 2);

                        result.Add (filter);
                        result.Add (action);
                    }

                    break;
                }
                case ActionType.CurrencyInitialise: {
                    if (simulation.CurrenciesValues.Keys.Any (c => c.ID == Currency.STATE)) {
                        string currencyState = localisation.Currencies[Currency.STATE];
                        sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == Currency.STATE)
                                .Select (k => k.Value)
                                .First ();

                        result.Add (StringLineFormatter.Indent ($"{currencyState} begins at {value}", 1));
                    }

                    SortedDictionary<sbyte, List<string>> currencyRegions = [];
                    string currencyRegion = string.Empty;

                    foreach (Faction region in simulation.Regions) {
                        if (simulation.CurrenciesValues.Any (k => k.Key.ID == region.ID)) {
                            sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == region.ID)
                                .Select (k => k.Value)
                                .First ();

                            currencyRegion = localisation.Currencies[region.ID];
                            
                            if (currencyRegions.TryGetValue (value, out var regions)) {
                                regions.Add (localisation.Regions[region.ID].Item1);
                            } else {
                                currencyRegions[value] = [localisation.Regions[region.ID].Item1];
                            }
                        }
                    }

                    if (currencyRegions.Count == 1) {
                        sbyte value = currencyRegions.Keys.First ();

                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Region.Item1}:", 1));
                        result.Add (StringLineFormatter.Indent ($"{currencyRegion} begins at {value}", 2));
                    } else if (currencyRegions.Count > 1) {
                        foreach (var pair in currencyRegions.Reverse ()) {
                            string regions = string.Join (", ", pair.Value);

                            result.Add (StringLineFormatter.Indent ($"{regions}:", 1));
                            result.Add (StringLineFormatter.Indent ($"{currencyRegion} begins at {pair.Key}:", 2));
                        }
                    }

                    SortedDictionary<sbyte, List<string>> currencyParties = [];
                    string currencyParty = string.Empty;

                    foreach (Faction party in simulation.Parties) {
                        if (simulation.CurrenciesValues.Any (k => k.Key.ID == party.ID)) {
                            sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == party.ID)
                                .Select (k => k.Value)
                                .First ();

                            currencyParty = localisation.Currencies[party.ID];

                            if (currencyRegions.TryGetValue (value, out var parties)) {
                                parties.Add (localisation.Regions[party.ID].Item1);
                            } else {
                                currencyRegions[value] = [localisation.Regions[party.ID].Item1];
                            }
                        }
                    }

                    if (currencyParties.Count == 1) {
                        sbyte value = currencyParties.Keys.First ();

                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Party.Item1}:", 1));
                        result.Add (StringLineFormatter.Indent ($"{currencyParty} begins at {value}", 2));
                    } else if (currencyParties.Count > 1) {
                        foreach (var pair in currencyParties.Reverse ()) {
                            string parties = string.Join (", ", pair.Value);

                            result.Add (StringLineFormatter.Indent ($"{parties}:", 1));
                            result.Add (StringLineFormatter.Indent ($"{currencyParty} begins at {pair.Key}:", 2));
                        }
                    }

                    break;
                }
                case ActionType.ProcedureActivate: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string target = TargetToString (this, in localisation);
                    string action = StringLineFormatter.Indent ($"Hold new {target}", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.ElectionRegion: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Randomly aligns with a {localisation.Region.Item1}", 2);

                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case ActionType.ElectionParty: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Randomly aligns with a {localisation.Party.Item1}", 2);

                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case ActionType.ElectionNominated: {
                    string target = TargetToString (this, in localisation);

                    result.Add (StringLineFormatter.Indent ($"Election:", 1));
                    result.Add (StringLineFormatter.Indent ($"Nominate three candidates for {target}", 2));
                    result.Add (StringLineFormatter.Indent ($"Division of chamber", 2));
                    break;
                }
                case ActionType.ElectionAppointed: {
                    string target = TargetToString (this, in localisation);

                    result.Add (StringLineFormatter.Indent ($"Appoints a {target}", 1));
                    break;
                }
                case ActionType.VotersLimit: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action = StringLineFormatter.Indent ("Cannot vote", 3);

                    result.Add (filter);
                    result.Add (target);
                    result.Add (action);
                    break;
                }
            }

            return string.Join ('\n', result);
        }
    }

    // TODO: remove ProcedureImmediate after done with effects
    internal readonly struct EffectBundle (IDType? procedureId = null, Confirmation? confirmation = null, byte value = 0, params Effect[] effects) {
        // Presence indicates ProcedureImmediate
        public IDType? ProcedureID => procedureId;
        public Effect[] Effects => effects;
        // Presence indicates ProcedureDeclared
        public Confirmation? Confirmation => confirmation;
        public byte Value => value;
    }

    public IDType ID => id;
    public Effect[] Effects => effects;
    public bool IsActiveStart => isActiveStart;

    public abstract EffectBundle? YieldEffects (ref readonly SimulationContext context);
    public abstract string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation);
}

/*
 * Activated once only at the beginning of a simulation
 * It is immediately deactivated after YieldEffect
 * It can only be activated again through a Procedure
 *
 * action: Election*
 */
internal class ProcedureImmediate : Procedure {
    public ProcedureImmediate (
        IDType id,
        Effect[] effects
    ) : base (id, effects) {
        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.ElectionRegion:
                case Effect.ActionType.ElectionParty:
                case Effect.ActionType.ElectionNominated:
                case Effect.ActionType.ElectionAppointed:
                    break;
                default:
                    throw new ArgumentException ($"ProcedureImmediate ID {id}: Action must be Election*");
            }
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (ID, effects: Effects);

    public override string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];

        foreach (Effect e in Effects) {
            string effect = e.ToString (in simulation, in localisation);

            if (e.Action is Effect.ActionType.ElectionAppointed) {
                effect = effect.Replace ("A", "Randomly a");
            }

            result.Add (effect);
        }

        return string.Join ('\n', result);
    }
}

/*
 * Activates on targeted Ballots
 * filter controls on which ballots it activates
 * filterIDs only matters if filter is TargetType.Only or TargetType.Except
 *
 * action: Vote*, Currency*, ProcedureActivate
 * filter: Ballot
 */
internal class ProcedureTargeted : Procedure {
    public ProcedureTargeted (
        IDType id,
        Effect[] effects
    ) : base (id, effects) {
        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.VotePassAdd:
                case Effect.ActionType.VoteFailAdd:
                case Effect.ActionType.VotePassTwoThirds:
                case Effect.ActionType.CurrencyAdd:
                case Effect.ActionType.CurrencySubtract:
                case Effect.ActionType.CurrencyInitialise:
                    break;
                case Effect.ActionType.ProcedureActivate:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureTargeted ID {id}: ProcedureActivate Target must be populated");
                    }

                    break;
                default:
                    throw new ArgumentException ($"ProcedureTargeted ID {id}: Action must be Vote*, Currency*, or ProcedureActivate");
            }
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) {
        List<Effect> effects = [];

        foreach (Effect e in Effects) {
            if (e.FilterIDs.Length == 0 || e.FilterIDs.Contains (context.BallotCurrentID)) {
                effects.Add (e);
            }
        }

        return effects.Count > 0 ? new EffectBundle(effects: effects.ToArray ()) : null;
    }

    public override string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in simulation, in localisation));
        }

        return string.Join ('\n', result);
    }
}

/*
 * Activates declaratively
 * Empty confirmationIDs means it activates on every Ballot
 *
 * action: Election*, VotersLimit
 * filter: Role (declarer)
 */
internal class ProcedureDeclared : Procedure {
    private readonly Confirmation _confirmation;
    private byte _value;
    // Filters Roles (empty: every, populated: specified)
    public IDType[] DeclarerIDs { get; }

    public ProcedureDeclared (
        IDType id,
        Effect[] effects,
        Confirmation confirmation,
        byte value,
        IDType[] declarerIDs
    ) : base (id, effects) {
        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.ElectionRegion:
                case Effect.ActionType.ElectionParty:
                case Effect.ActionType.VotersLimit:
                    break;
                case Effect.ActionType.ElectionNominated:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: ElectionNominated Target must be populated");
                    }

                    break;
                case Effect.ActionType.ElectionAppointed:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: ElectionAppointed Target must be populated");
                    }

                    break;
                default:
                    throw new ArgumentException ($"ProcedureDeclared ID {id}: Action must be Election* or VotersLimit");
            }
        }

        _confirmation = confirmation;
        _value = value;
        DeclarerIDs = declarerIDs;
    }

    private string DeclarerToString (ref readonly Localisation localisation) {
        return DeclarerIDs.Length > 0
            ? string.Join (
                ", ",
                localisation.Roles.Where (k => DeclarerIDs.Contains (k.Key))
                    .Select (k => k.Value.Item2)
            ) + ":"
            : "Everyone:";
    }

    private string ConfirmationToString (ref readonly Localisation localisation) {
        switch (_confirmation.Cost) {
            case Confirmation.CostType.Always:
                return "Always";
            case Confirmation.CostType.DivisionChamber:
                return "Division of chamber";
            case Confirmation.CostType.CurrencyValue: {
                HashSet<string> currencies = [];

                foreach (IDType d in DeclarerIDs) {
                    if (
                        d == Role.MEMBER
                        || d == Role.HEAD_GOVERNMENT
                        || d == Role.HEAD_STATE
                    ) {
                        currencies.Add (localisation.Currencies[Currency.STATE]);
                    } else if (d == Role.LEADER_PARTY) {
                        currencies.Add (localisation.Currencies[Currency.PARTY]);
                    } else if (d == Role.LEADER_REGION) {
                        currencies.Add (localisation.Currencies[Currency.REGION]);
                    } else {
                        currencies.Add (localisation.Currencies[d]);
                    }
                }

                return $"Can subtract {_confirmation.Value} from {string.Join (", ", currencies)}";
            }
            case Confirmation.CostType.SingleDiceValue:
                return $"Dice roll greater than or equal to {_confirmation.Value}";
            case Confirmation.CostType.SingleDiceCurrency: {
                HashSet<string> currencies = [];

                foreach (IDType d in DeclarerIDs) {
                    if (
                        d == Role.MEMBER
                        || d == Role.HEAD_GOVERNMENT
                        || d == Role.HEAD_STATE
                    ) {
                        currencies.Add (localisation.Currencies[Currency.STATE]);
                    } else if (d == Role.LEADER_PARTY) {
                        currencies.Add (localisation.Currencies[Currency.PARTY]);
                    } else if (d == Role.LEADER_REGION) {
                        currencies.Add (localisation.Currencies[Currency.REGION]);
                    } else {
                        currencies.Add (localisation.Currencies[d]);
                    }
                }

                return $"Can subtract dice roll from {string.Join (", ", currencies)}";
            }
            default:
                throw new UnreachableException ();
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (null, _confirmation, _value, Effects);

    public override string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];
        string declarer = StringLineFormatter.Indent (DeclarerToString (in localisation), 1);
        string canDeclare = StringLineFormatter.Indent ("Can declare if:", 2);
        string confirmation = StringLineFormatter.Indent (ConfirmationToString (in localisation), 3);

        result.Add (declarer);
        result.Add (canDeclare);
        result.Add (confirmation);

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in simulation, in localisation));
        }

        return string.Join ('\n', result);
    }
}
