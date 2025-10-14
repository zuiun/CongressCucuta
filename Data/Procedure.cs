using System.Diagnostics;
using congress_cucuta.Converters;

namespace congress_cucuta.Data;

internal abstract class Procedure (
    IDType id,
    Procedure.Effect[] effects,
    bool isActiveStart = true
): IID {
    // Presence indicates ProcedureDeclared
    internal readonly record struct Confirmation (Confirmation.CostType Cost, byte Value = 0) {
        /*
         * Always: always succeeds
         * DivisionChamber: simple majority vote, succeeds if vote passes
         * CurrencyValue: succeeds if Currency is higher than Value, Value is subtracted from Currency
         * DiceValue: rolls one dice, succeeds if dice >= Value
         * DiceCurrency: rolls one dice, succeeds if dice <= Currency, dice is subtracted from Currency
         * DiceAdversarial: rolls two die representing declarer and other,
         * succeeds if declarer's dice >= other's dice,
         * declarer's dice is subtracted from declarer's Currency if present
         */
        internal enum CostType {
            Always,
            DivisionChamber,
            CurrencyValue,
            DiceValue,
            DiceCurrency,
            DiceAdversarial,
        }
    }

    internal readonly record struct Effect (
        Effect.ActionType Action,
        IDType[] TargetIDs,
        byte Value = 0
    ) {
        internal enum ActionType {
            /*
             * Adds Value votes for the passage of Filter Ballot
             *
             * Targeted
             */
            VotePassAdd,
            /*
             * Adds Value votes for the failure of Filter Ballot
             *
             * Targeted
             */
            VoteFailAdd,
            /*
             * Causes Filter Ballot to require a two-thirds majority to pass
             *
             * Targeted
             */
            VotePassTwoThirds,
            /*
             * Adds Value to Target Currency during Filter Ballot
             *
             * Targeted, Declared
             * Targets Factions (populated: specified), Currencies (empty: STATE or declarer)
             */
            CurrencyAdd,
            /*
             * Subtracts Value from Target Currency during Filter Ballot
             *
             * Targeted, Declared
             * Targets Factions (populated: specified), Currencies (empty: STATE or declarer)
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
             * Activates Target ProcedureImmediate during Filter Ballot
             *
             * Targeted
             * Targets Procedure (populated: specified)
             */
            ProcedureActivate,
            /*
             * Assigns Target Roles to Regions
             * Elects LEADER_REGION if present
             *
             * Immediate, Declared
             * Targets Roles (populated: excluded)
             * Value (0: choose LEADER_REGION, anything else: random LEADER_REGION)
             */
            ElectionRegion,
            /*
             * Assigns Target Roles to Parties
             * Elects LEADER_PARTY if present
             *
             * Immediate, Declared
             * Targets Roles (populated: excluded)
             * Value (0: choose LEADER_PARTY, anything else: random LEADER_PARTY)
             */
            ElectionParty,
            /*
             * Elects Target Role
             *
             * Immediate, Declared
             * Targets Roles (populated: specified [first], excluded [remainder])
             */
            ElectionNominated,
            /*
             * Appoints Target Role
             *
             * Immediate, Declared
             * Targets Roles (populated: specified [first], excluded [remainder])
             * Value (0: choose Target, anything else: random Target)
             */
            ElectionAppointed,
            /*
             * Only allows Target Roles or Factions to vote on current Ballot
             *
             * Declared
             * Targets Roles and Factions (empty: declarer, populated: specified [whichever is found at that ID, with Faction taking priority])
             */
            BallotLimit,
            /*
             * Immediately passes current Ballot
             *
             * Declared
             */
            BallotPass,
            /*
             * Immediately fails current Ballot
             *
             * Declared
             */
            BallotFail,
            /*
             * Sets Target Role voting Permissions to Value during Filter Ballot (when Targeted)
             *
             * Immediate, Targeted
             * Targets Roles (empty: random [person, only Targeted], populated: specified)
             * Value (0: false, anything else: true)
             */
            PermissionsCanVote,
            /*
             * Sets Target Role votes Permissions to Value during Filter Ballot (when Targeted)
             *
             * Immediate, Targeted
             * Targets Roles (empty: random [person, only Targeted], populated: specified)
             */
            PermissionsVotes,
            /*
             * Sets Target Role speaking Permissions to Value during Filter Ballot (when Targeted)
             *
             * Immediate, Targeted
             * Targets Roles (empty: random [person, only Targeted], populated: specified)
             * Value (0: false, anything else: true)
             */
            PermissionsCanSpeak,
        }

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
                case ActionType.ElectionAppointed: {
                    string target = localisation.Roles[effect.TargetIDs[0]].Item1;

                    if (effect.TargetIDs.Length > 1) {
                        List<string> excluded = [];

                        for (byte i = 1; i < effect.TargetIDs.Length; ++ i) {
                            excluded.Add (localisation.Roles[effect.TargetIDs[i]].Item2);
                        }

                        return $"{target}\nEveryone except {string.Join (", ", excluded)}:";
                    } else {
                        return $"{target}\nEveryone:";
                    }
                }
                case ActionType.BallotLimit: {
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
                case ActionType.PermissionsCanVote:
                case ActionType.PermissionsVotes:
                case ActionType.PermissionsCanSpeak: {
                    if (effect.TargetIDs.Length > 0) {
                        List<string> specified = [];

                        foreach (IDType t in effect.TargetIDs) {
                            specified.Add (localisation.Roles[t].Item2);
                        }

                        return $"{string.Join (", ", specified)}:";
                    } else {
                        return "Random person:";
                    }
                }
                default:
                    throw new NotSupportedException ();
            };
        }

        public string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
            List<string> result = [];

            switch (Action) {
                case ActionType.VotePassAdd: {
                    string action = StringLineFormatter.Indent ($"Gains {Value} vote(s) in favour", 2);

                    result.Add (action);
                    break;
                }
                case ActionType.VoteFailAdd: {
                    string action = StringLineFormatter.Indent ($"Gains {Value} vote(s) in opposition", 2);

                    result.Add (action);
                    break;
                }
                case ActionType.VotePassTwoThirds: {
                    string action = StringLineFormatter.Indent ($"Needs a two-thirds majority to pass", 2);

                    result.Add (action);
                    break;
                }
                case ActionType.CurrencyAdd: {
                    if (TargetIDs.Length > 0) {
                        string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                        string action = StringLineFormatter.Indent ($"Gains {Value} {localisation.Currencies[TargetIDs[0]]}", 3);

                        result.Add (target);
                        result.Add (action);
                    } else {
                        string action = StringLineFormatter.Indent ($"Gain {Value} {localisation.Currencies[Currency.STATE]}", 2);

                        result.Add (action);
                    }

                    break;
                }
                case ActionType.CurrencySubtract: {
                    if (TargetIDs.Length > 0) {
                        string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                        string action = StringLineFormatter.Indent ($"Loses {Value} {localisation.Currencies[TargetIDs[0]]}", 3);

                        result.Add (target);
                        result.Add (action);
                    } else {
                        string action = StringLineFormatter.Indent ($"Lose {Value} {localisation.Currencies[Currency.STATE]}", 2);

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

                    SortedList<sbyte, List<string>> currencyRegions = [];
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

                    SortedList<sbyte, List<string>> currencyParties = [];
                    string currencyParty = string.Empty;

                    foreach (Faction party in simulation.Parties) {
                        if (simulation.CurrenciesValues.Any (k => k.Key.ID == party.ID)) {
                            sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == party.ID)
                                .Select (k => k.Value)
                                .First ();

                            currencyParty = localisation.Currencies[party.ID];

                            if (currencyParties.TryGetValue (value, out var parties)) {
                                parties.Add (localisation.Parties[party.ID].Item1);
                            } else {
                                currencyParties[value] = [localisation.Parties[party.ID].Item1];
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
                    string target = TargetToString (this, in localisation);
                    string action = StringLineFormatter.Indent ($"Hold new {target}", 2);

                    result.Add (action);
                    break;
                }
                case ActionType.ElectionRegion: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Randomly aligns with a {localisation.Region.Item1}", 2);
                    bool isRandom = Value > 0;

                    result.Add (target);
                    result.Add (action);

                    if (localisation.Roles.TryGetValue (Role.LEADER_REGION, out (string, string) leader)) {
                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Region.Item1}:", 1));

                        if (isRandom) {
                            result.Add (StringLineFormatter.Indent ($"Randomly appoints {leader.Item1}", 2));
                        } else {
                            result.Add (StringLineFormatter.Indent ($"Elects {leader.Item1}", 2));
                        }
                    }

                    break;
                }
                case ActionType.ElectionParty: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Randomly aligns with a {localisation.Party.Item1}", 2);
                    bool isRandom = Value > 0;

                    result.Add (target);
                    result.Add (action);

                    if (localisation.Roles.TryGetValue (Role.LEADER_PARTY, out (string, string) leader)) {
                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Party.Item1}:", 1));

                        if (isRandom) {
                            result.Add (StringLineFormatter.Indent ($"Randomly appoints {leader.Item1}", 2));
                        } else {
                            result.Add (StringLineFormatter.Indent ($"Elects {leader.Item1}", 2));
                        }
                    }

                    break;
                }
                case ActionType.ElectionNominated: {
                    string[] targets = TargetToString (this, in localisation).Split ('\n');
                    string target = targets[0];
                    string candidates = targets[1];

                    result.Add (StringLineFormatter.Indent ($"Elect {target}:", 1));
                    result.Add (StringLineFormatter.Indent (candidates, 2));
                    result.Add (StringLineFormatter.Indent ("Can be nominated", 3));
                    break;
                }
                case ActionType.ElectionAppointed: {
                    string[] targets = TargetToString (this, in localisation).Split ('\n');
                    string target = targets[0];
                    string candidates = targets[1];
                    bool isRandom = Value > 0;

                    if (isRandom) {
                        result.Add (StringLineFormatter.Indent ($"Randomly appoint {target}:", 1));
                    } else {
                        result.Add (StringLineFormatter.Indent ($"Appoint {target}:", 1));
                    }

                    result.Add (StringLineFormatter.Indent (candidates, 2));
                    result.Add (StringLineFormatter.Indent ("Can be nominated", 3));
                    break;
                }
                case ActionType.BallotLimit: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action = StringLineFormatter.Indent ("Cannot vote", 3);

                    result.Add (filter);
                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case ActionType.BallotPass: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string action = StringLineFormatter.Indent ("Immediately passes", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.BallotFail: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string action = StringLineFormatter.Indent ("Immediately fails", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.PermissionsCanVote: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action;

                    if (Value > 0) {
                        action = StringLineFormatter.Indent ("Can vote", 3);
                    } else {
                        action = StringLineFormatter.Indent ("Cannot vote", 3);
                    }

                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case ActionType.PermissionsVotes: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action = StringLineFormatter.Indent ($"Gain {Value} vote(s)", 3);

                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case ActionType.PermissionsCanSpeak: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action;

                    if (Value > 0) {
                        action = StringLineFormatter.Indent ("Can speak", 3);
                    } else {
                        action = StringLineFormatter.Indent ("Cannot speak", 3);
                    }

                    result.Add (target);
                    result.Add (action);
                    break;
                }
            }

            return string.Join ('\n', result);
        }
    }

    internal readonly record struct EffectBundle (
        Effect[] Effects,
        Confirmation? Confirmation = null,
        byte Value = 0
    );

    public IDType ID => id;
    public Effect[] Effects => effects;
    public bool IsActiveStart => isActiveStart;

    public abstract EffectBundle? YieldEffects (IDType ballotId);
    public abstract string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation);
}

/*
 * Activated once at the beginning of a simulation
 * It can only be activated again through a Procedure
 *
 * effects: populated
 * action: Election*, Permissions*
 */
internal class ProcedureImmediate : Procedure {
    public ProcedureImmediate (IDType id, Effect[] effects) : base (id, effects) {
        if (effects.Length == 0) {
            throw new ArgumentException ($"ProcedureImmediate ID {id}: ProcedureImmediate must have Effect", nameof (effects));
        }
        
        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.ElectionRegion:
                case Effect.ActionType.ElectionParty:
                case Effect.ActionType.ElectionNominated:
                case Effect.ActionType.ElectionAppointed:
                    break;
                case Effect.ActionType.PermissionsCanVote:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureImmediate ID {id}: PermissionsCanSpeak Target must be populated", nameof (effects));
                    }

                    break;
                case Effect.ActionType.PermissionsVotes:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureImmediate ID {id}: PermissionsCanSpeak Target must be populated", nameof (effects));
                    }

                    break;
                case Effect.ActionType.PermissionsCanSpeak:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureImmediate ID {id}: PermissionsCanSpeak Target must be populated", nameof (effects));
                    }

                    break;
                default:
                    throw new ArgumentException ($"ProcedureImmediate ID {id}: Action must be Election* or Permissions*");
            }
        }
    }

    public override EffectBundle? YieldEffects (IDType ballotId) => new (Effects);

    public override string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];

        foreach (Effect e in Effects) {
            string effect = e.ToString (in simulation, in localisation);

            if (
                e.Action is Effect.ActionType.PermissionsCanVote
                or Effect.ActionType.PermissionsVotes
                or Effect.ActionType.PermissionsCanSpeak
            ) {
                effect = StringLineFormatter.Outdent (effect);
            }

            result.Add (effect);
        }

        return string.Join ('\n', result);
    }
}

/*
 * Activates on filtered Ballots
 *
 * effects: at least one
 * action: Vote*, Currency*, ProcedureActivate, Permissions*
 * filter: Ballot
 */
internal class ProcedureTargeted : Procedure {
    // Filters Ballots (empty: every, populated: specified)
    public IDType[] BallotIDs { get; }

    public ProcedureTargeted (
        IDType id,
        Effect[] effects,
        IDType[] ballotIds,
        bool isActiveStart = true
    ) : base (id, effects, isActiveStart) {
        if (effects.Length == 0) {
            throw new ArgumentException ($"ProcedureTargeted ID {id}: ProcedureTargeted must have Effect");
        }

        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.VotePassAdd:
                case Effect.ActionType.VoteFailAdd:
                case Effect.ActionType.VotePassTwoThirds:
                case Effect.ActionType.CurrencyAdd:
                case Effect.ActionType.CurrencySubtract:
                case Effect.ActionType.CurrencyInitialise:
                case Effect.ActionType.PermissionsCanVote:
                case Effect.ActionType.PermissionsVotes:
                case Effect.ActionType.PermissionsCanSpeak:
                    break;
                case Effect.ActionType.ProcedureActivate:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureTargeted ID {id}: ProcedureActivate Target must be populated");
                    }

                    break;
                default:
                    throw new ArgumentException ($"ProcedureTargeted ID {id}: Action must be Vote*, Currency*, ProcedureActivate, or Permissions*");
            }
        }

        BallotIDs = ballotIds;
    }

    private string FilterToString (ref readonly Localisation localisation) {
        if (BallotIDs.Length > 0) {
            var ballotsIter = localisation.Ballots.Where (k => BallotIDs.Contains (k.Key))
                .Select (k => k.Value.Item1);

            return $"{string.Join (", ", ballotsIter)}:";
        } else {
            return "Every Ballot:";
        }
    }

    public override EffectBundle? YieldEffects (IDType ballotId) =>
        (BallotIDs.Length == 0 || BallotIDs.Contains (ballotId)) ? new EffectBundle (Effects) : null;

    public override string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];
        string filter = StringLineFormatter.Indent (FilterToString (in localisation), 1);
        bool isFilterAdded = false;

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in simulation, in localisation));

            if (e.Action is Effect.ActionType.CurrencyInitialise) {
                if (Effects.Length > 1) {
                    result.Add (filter);
                }

                isFilterAdded = true;
            }
        }

        if (! isFilterAdded) {
            result.Insert (1, filter);
        }

        return string.Join ('\n', result);
    }
}

/*
 * Activates declaratively
 *
 * effects: at least one
 * action: Election*, Ballot*
 * declarerIds: Role (declarer)
 */
internal class ProcedureDeclared : Procedure {
    public Confirmation Confirm { get; }
    public byte Value { get; }
    // Filters Roles (empty: every, populated: specified)
    public IDType[] DeclarerIDs { get; }

    public ProcedureDeclared (
        IDType id,
        Effect[] effects,
        Confirmation confirm,
        byte value,
        IDType[] declarerIds
    ) : base (id, effects) {
        if (effects.Length == 0) {
            throw new ArgumentException ($"ProcedureDeclared ID {id}: ProcedureDeclared must have Effect", nameof (effects));
        }

        bool isPass = false;
        bool isFail = false;

        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.CurrencyAdd:
                case Effect.ActionType.CurrencySubtract:
                case Effect.ActionType.ElectionRegion:
                case Effect.ActionType.ElectionParty:
                case Effect.ActionType.BallotLimit:
                    break;
                case Effect.ActionType.BallotPass:
                    if (isFail) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: Cannot have both BallotPass and BallotFail");
                    } else {
                        isPass = true;
                    }

                    break;
                case Effect.ActionType.BallotFail:
                    if (isPass) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: Cannot have both BallotPass and BallotFail");
                    } else {
                        isFail = true;
                    }

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

        Confirm = confirm;
        Value = value;
        DeclarerIDs = declarerIds;
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
        switch (Confirm.Cost) {
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

                return $"Can spend {Confirm.Value} from {string.Join (", ", currencies)}";
            }
            case Confirmation.CostType.DiceValue:
                return $"Dice roll greater than or equal to {Confirm.Value}";
            case Confirmation.CostType.DiceCurrency: {
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

                return $"Can spend dice roll from {string.Join (", ", currencies)}";
            }
            case Confirmation.CostType.DiceAdversarial: {
                HashSet<string> currencies = [];
                string dice = "Declarer's dice roll greater than or equal to defender's dice roll";

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

                if (localisation.Currencies.Count > 0) {
                    return $"Can spend declarer's dice roll from {string.Join (", ", currencies)}\n{dice}";
                } else {
                    return dice;
                }
            }
            default:
                throw new UnreachableException ();
        }
    }

    public override EffectBundle? YieldEffects (IDType ballotId) => new (Effects, Confirmation: Confirm, Value: Value);

    public override string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];
        string declarer = StringLineFormatter.Indent (DeclarerToString (in localisation), 1);
        string canDeclare = StringLineFormatter.Indent ("Can declare if:", 2);
        string[] confirmation = ConfirmationToString (in localisation).Split ('\n');

        result.Add (declarer);
        result.Add (canDeclare);
        result.AddRange (confirmation.Select (c => StringLineFormatter.Indent (c, 3)));

        foreach (Effect e in Effects) {
            string action = e.ToString (in simulation, in localisation);

            if (e.Action is Effect.ActionType.CurrencyAdd or Effect.ActionType.CurrencySubtract) {
                action = StringLineFormatter.Outdent (action);
            }

            result.Add (action);
        }

        return string.Join ('\n', result);
    }
}
