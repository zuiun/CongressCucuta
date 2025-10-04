using congress_cucuta.Converters;
using System.Diagnostics;
using static congress_cucuta.Data.Procedure.Effect;

namespace congress_cucuta.Data;

internal abstract class Procedure (
    IDType id,
    Procedure.Effect[] effects
): IID {
    /*
     * Presence indicates DeclaredProcedure
     *
     * Always: always succeeds
     * DivisionChamber: simple majority vote, succeeds if vote passes
     * CurrencyValue: succeeds if Currency is higher than Value, Value is subtracted from Currency
     * SingleDiceValue: rolls one dice, succeeds if dice >= Value
     * SingleDiceCurrency: rolls one dice, succeeds if dice <= Currency, dice is subtracted from Currency
     * AdversarialDice: rolls two die representing declarer and others, whichever dice is higher gets positive effects of Procedure
     */
    internal enum ConfirmationType {
        Always,
        DivisionChamber,
        CurrencyValue,
        SingleDiceValue,
        SingleDiceCurrency,
        // AdversarialDice,
    }

    internal readonly struct Effect (
        ActionType action,
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
             * Activates Target ProcedureImmediate
             *
             * Targeted
             * Targets Procedure (populated: specified)
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
             * Targets Roles (populated: specified)
             */
            ElectionAppointed,
            /*
             * Limits eligible voters on Target Ballot
             *
             * Declared
             * Targets Value (empty), Currencies (populated [ignored]: declarer's)
             */
            VotersLimit,
        }

        public ActionType Action => action;
        public byte Value => value;
        public IDType[] TargetIDs => targetIDs;
        public IDType[] FilterIDs => filterIDs;

        private static string TargetToString (Effect effect, ref readonly Localisation localisation) {
            throw new NotImplementedException ();
            string target = effect.Action switch {
                ActionType.CurrencyAdd
                or ActionType.CurrencySubtract => "",
                ActionType.ProcedureActivate => "",
                ActionType.ElectionRegion
                or ActionType.ElectionParty
                or ActionType.ElectionNominated
                or ActionType.ElectionAppointed =>
                    effect.FilterIDs.Length > 0
                        ? string.Join (
                            ", ",
                            localisation.Roles.Where (k => effect.FilterIDs.Contains (k.Key))
                                .Select (k => k.Value.Item2)
                        )
                        : "Everyone",
                ActionType.VotersLimit => "",
                _ => throw new NotSupportedException (),
            };
        }

        private static string FilterToString (Effect effect, ref readonly Localisation localisation) {
            return effect.Action switch {
                ActionType.VotePassAdd
                or ActionType.VoteFailAdd
                or ActionType.VotePassTwoThirds
                or ActionType.CurrencyAdd
                or ActionType.CurrencySubtract
                or ActionType.ProcedureActivate =>
                    effect.FilterIDs.Length > 0
                        ? string.Join (
                            ", ",
                            localisation.Ballots.Where (k => effect.FilterIDs.Contains (k.Key))
                                .Select (k => k.Value.Item1)
                        ) + ":"
                        : "Every Ballot:",
                _ => throw new NotSupportedException (),
            };
        }

        public string ToString (ref readonly Localisation localisation) {
            List<string> result = [];

            switch (Action) {
                case ActionType.VotePassAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Favour", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.VoteFailAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.VotePassTwoThirds: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Needs a Two-Thirds Majority to Pass", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.CurrencyAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    throw new NotImplementedException ();
                    // TODO: Get Currency name
                    // empty: STATE, populated: Faction Currency
                    string action = StringLineFormatter.Indent ($"Gains {Value} ", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.CurrencySubtract: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    throw new NotImplementedException ();
                    string action = StringLineFormatter.Indent ($"Loses {Value} ", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.ProcedureActivate: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in localisation), 1);
                    throw new NotImplementedException ();
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                }
                case ActionType.ElectionRegion: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    //string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.RegionSingular}", 2);

                    //result.Add (action);
                }
                case ActionType.ElectionParty: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    //string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.PartySingular}", 2);

                    //result.Add (action);
                }
                case ActionType.ElectionNominated: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    //string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.PartySingular}", 1);

                    //result.Add (action);
                }
                case ActionType.ElectionAppointed: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    //string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.PartySingular}", 1);

                    //result.Add (action);
                }
                case ActionType.VotersLimit: {
                    throw new NotImplementedException ();
                }
            }

            return string.Join ('\n', result);
        }
    }

    // TODO: remove ProcedureImmediate after done with effects
    internal readonly struct EffectBundle (IDType? procedureId = null, ConfirmationType? confirmation = null, byte value = 0, params Effect[] effects) {
        // Presence indicates ProcedureImmediate
        public IDType? ProcedureID => procedureId;
        public Effect[] Effects => effects;
        public ConfirmationType? Confirmation => confirmation;
        public byte Value => value;
    }

    public IDType ID => id;
    public Effect[] Effects => effects;

    public abstract EffectBundle? YieldEffects (ref readonly SimulationContext context);
    public abstract string ToString (ref readonly Localisation localisation);
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
                case ActionType.ElectionRegion:
                case ActionType.ElectionParty:
                case ActionType.ElectionNominated:
                case ActionType.ElectionAppointed:
                    break;
                default:
                    throw new ArgumentException ($"ProcedureImmediate ID {id}: Action must be Election*");
            }
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (ID, effects: Effects);

    public override string ToString (ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in localisation));
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
                case ActionType.VotePassAdd:
                case ActionType.VoteFailAdd:
                case ActionType.VotePassTwoThirds:
                case ActionType.CurrencyAdd:
                case ActionType.CurrencySubtract:
                    break;
                case ActionType.ProcedureActivate:
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

    public override string ToString (ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in localisation));
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
    private readonly ConfirmationType? _confirmation;
    private byte _value;
    // Filters Roles (empty: every, populated: specified)
    public IDType[] DeclarerIDs { get; }

    public ProcedureDeclared (
        IDType id,
        Effect[] effects,
        ConfirmationType? confirmation,
        byte value,
        IDType[] declarerIDs
    ) : base (id, effects) {
        foreach (Effect e in effects) {
            switch (e.Action) {
                case ActionType.ElectionRegion:
                case ActionType.ElectionParty:
                case ActionType.VotersLimit:
                    break;
                case ActionType.ElectionNominated:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: ElectionNominated Target must be populated");
                    }

                    break;
                case ActionType.ElectionAppointed:
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

    /*
     * Presence indicates DeclaredProcedure
     *
     * CurrencyValue: succeeds if Currency is higher than Value, Value is subtracted from Currency
     * SingleDiceValue: rolls one dice, succeeds if dice is higher than Value
     * SingleDiceCurrency: rolls one dice, succeeds if Currency is higher than dice, dice is subtracted from Currency
     */
    private string ConfirmationToString (ref readonly Localisation localisation) {
        switch (_confirmation) {
            case ConfirmationType.Always:
                return "Always";
            case ConfirmationType.DivisionChamber:
                return "Division of Chamber";
            case ConfirmationType.CurrencyValue: {
                // TODO: Get Currency name via Declarer ID (should be a Role)
                throw new NotImplementedException ();
                return $"Can Subtract {_value} from {localisation.Currencies}";
            }
            case ConfirmationType.SingleDiceValue:
                return $"Dice Roll Greater than or Equal to {_value}";
            case ConfirmationType.SingleDiceCurrency: {
                // TODO: Get Currency name via Declarer ID (should be a Role)
                throw new NotImplementedException ();
                return $"Can Subtract Dice Roll from {localisation.Currencies}";
            }
            default:
                throw new UnreachableException ();
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (null, _confirmation, _value, Effects);

    public override string ToString (ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];
        string declarer = StringLineFormatter.Indent (DeclarerToString (in localisation), 1);
        string canDeclare = StringLineFormatter.Indent ("Can declare if:", 2);
        string confirmation = StringLineFormatter.Indent (ConfirmationToString (in localisation), 3);

        result.Add (declarer);
        result.Add (canDeclare);
        result.Add (confirmation);

        foreach (Effect e in Effects) {
            string effect = StringLineFormatter.Indent (e.ToString (in localisation), 1);

            result.Add (effect);
        }

        return string.Join ('\n', result);
    }
}
