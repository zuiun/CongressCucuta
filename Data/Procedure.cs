using System.Diagnostics;
using congress_cucuta.Converters;

namespace congress_cucuta.Data;

internal abstract class Procedure (
    IDType id,
    string name,
    string description,
    Procedure.Effect[] effects
): IID {
    /*
     * Presence indicates DeclaredProcedure
     *
     * Always: always succeeds
     * DivisionChamber: simple majority vote, succeeds if vote passes
     * CurrencyValue: succeeds if Currency is higher than Value, Value is subtracted from Currency
     * SingleDiceValue: rolls one dice, succeeds if dice is higher than Value
     * SingleDiceCurrency: rolls one dice, succeeds if Currency is higher than dice, dice is subtracted from Currency
     * AdversarialDice: rolls two die representing declarer and others, whichever dice is higher gets positive effects of Procedure
     */
    internal enum ConfirmationType {
        Always,
        DivisionChamber,
        SingleDiceValue,
        SingleDiceCurrency,
        AdversarialDice,
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
             * Targets Currencies (populated: specified [Faction ID, PARTY, REGION, STATE])
             * Filters Ballots (empty: every, populated: specified)
             */
            CurrencyAdd,
            /*
             * Subtracts Value from Target Currency
             *
             * Targeted
             * Targets Currencies (populated: specified [Faction ID, PARTY, REGION, STATE])
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
             * Targets Value (empty) Currencies (populated [ignored]: declarer's)
             */
            VotersLimit,
        }

        public ActionType Action => action;
        public byte Value => value;
        public IDType[] TargetIDs => targetIDs;
        public IDType[] FilterIDs => filterIDs;

        private static string TargetToString (Effect effect, ref readonly SimulationContext context) {
            throw new NotImplementedException ();
            string target = effect.Action switch {
                ActionType.ElectionRegion
                or ActionType.ElectionParty
                or ActionType.ElectionNominated
                or ActionType.ElectionAppointed
                or ActionType.VotersLimit =>
                    effect.FilterIDs.Length > 0
                        ? string.Join (
                            ", ",
                            context.Roles.Values.Where (r => effect.FilterIDs.Contains (r.ID))
                                .Select (r => r.TitlePlural)
                        )
                        : "Everyone",
                _ => throw new NotSupportedException (),
            };
        }

        private static string FilterToString (Effect effect, ref readonly SimulationContext context) {
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
                            context.Ballots.Values.Where (b => effect.FilterIDs.Contains (b.ID))
                                .Select (b => b.Title)
                        ) + ":"
                        : "Every Ballot:",
                ActionType.ElectionRegion
                or ActionType.ElectionParty
                or ActionType.ElectionNominated
                or ActionType.ElectionAppointed
                or ActionType.VotersLimit =>
                    effect.FilterIDs.Length > 0
                        ? string.Join (
                            ", ",
                            context.Roles.Values.Where (r => effect.FilterIDs.Contains (r.ID))
                                .Select (r => r.TitlePlural)
                        ) + ":"
                        : "Everyone:",
                _ => throw new NotSupportedException (),
            };
        }

        public string ToString (ref readonly SimulationContext context) {
            List<string> result = [];

            switch (Action) {
                case ActionType.VotePassAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in context), 1);
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Favour", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.VoteFailAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in context), 1);
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.VotePassTwoThirds: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in context), 1);
                    string action = StringLineFormatter.Indent ($"Needs a Two-Thirds Majority to Pass", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.CurrencyAdd: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in context), 1);
                    throw new NotImplementedException ();
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.CurrencySubtract: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in context), 1);
                    throw new NotImplementedException ();
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case ActionType.ProcedureActivate: {
                    string filter = StringLineFormatter.Indent (FilterToString (this, in context), 1);
                    throw new NotImplementedException ();
                    string action = StringLineFormatter.Indent ($"Gains {Value} Vote(s) in Opposition", 2);

                    result.Add (filter);
                    result.Add (action);
                }
                case ActionType.ElectionRegion: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.RegionSingular}", 2);

                    result.Add (action);
                }
                case ActionType.ElectionParty: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.PartySingular}", 2);

                    result.Add (action);
                }
                case ActionType.ElectionNominated: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.PartySingular}", 1);

                    result.Add (action);
                }
                case ActionType.ElectionAppointed: {
                    throw new NotImplementedException ();
                    // TODO: target?
                    string action = StringLineFormatter.Indent ($"Aligns randomly with a {context.PartySingular}", 1);

                    result.Add (action);
                }
                case ActionType.VotersLimit: {
                    throw new NotImplementedException ();
                }
            }

            return string.Join ('\n', result);
        }
    }

    // TODO: remove ProcedureImmediate after done with effects
    internal readonly struct EffectBundle (IDType? procedureId = null, ConfirmationType? confirmation = null, params Effect[] effects) {
        // Presence indicates ProcedureImmediate
        public IDType? ProcedureID => procedureId;
        public Effect[] Effects => effects;
        public ConfirmationType? Confirmation => confirmation;
    }

    public IDType ID { get; } = id;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public Effect[] Effects { get; } = effects;

    public abstract EffectBundle? YieldEffects (ref readonly SimulationContext context);
    public abstract string ToString (ref readonly SimulationContext context);
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
        string name,
        string description,
        Effect[] effects
    ) : base (id, name, description, effects) {
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

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (ID, null, Effects);

    public override string ToString (ref readonly SimulationContext context) {
        List<string> result = [StringLineFormatter.Indent (Name, 0)];

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in context));
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
        string name,
        string description,
        Effect[] effects
    ) : base (id, name, description, effects) {
        foreach (Effect e in effects) {
            switch (e.Action) {
                case Effect.ActionType.VotePassAdd:
                case Effect.ActionType.VoteFailAdd:
                case Effect.ActionType.VotePassTwoThirds:
                case Effect.ActionType.CurrencyAdd:
                case Effect.ActionType.CurrencySubtract:
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

        return effects.Count > 0 ? new EffectBundle(null, null, effects.ToArray ()) : null;
    }

    public override string ToString (ref readonly SimulationContext context) {
        List<string> result = [StringLineFormatter.Indent (Name, 0)];

        foreach (Effect e in Effects) {
            result.Add (e.ToString (in context));
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
    // Filters Roles (empty: every, populated: specified)
    public IDType[] DeclarerIDs { get; }

    public ProcedureDeclared (
        IDType id,
        string name,
        string description,
        Effect[] effects,
        ConfirmationType? confirmation,
        IDType[] declarerIDs
    ) : base (id, name, description, effects) {
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
        DeclarerIDs = declarerIDs;
    }

    private string DeclarerToString (ref readonly SimulationContext context) {
        throw new NotImplementedException ();
        //return effect.Action switch {
        //    Effect.ActionType.VotePassAdd
        //    or Effect.ActionType.VoteFailAdd
        //    or Effect.ActionType.VotePassTwoThirds
        //    or Effect.ActionType.CurrencyAdd
        //    or Effect.ActionType.CurrencySubtract
        //    or Effect.ActionType.ProcedureActivate =>
        //        effect.FilterIDs.Length > 0
        //            ? string.Join (
        //                ", ",
        //                context.Ballots.Values.Where (b => effect.FilterIDs.Contains (b.ID))
        //                    .Select (b => b.Title)
        //            ) + ":"
        //            : "Every Ballot:",
        //    Effect.ActionType.ElectionRegion
        //    or Effect.ActionType.ElectionParty
        //    or Effect.ActionType.ElectionNominated
        //    or Effect.ActionType.ElectionAppointed
        //    or Effect.ActionType.VotersLimit =>
        //        effect.FilterIDs.Length > 0
        //            ? string.Join (
        //                ", ",
        //                context.Roles.Values.Where (r => effect.FilterIDs.Contains (r.ID))
        //                    .Select (r => r.TitlePlural)
        //            ) + ":"
        //            : "Everyone:",
        //    _ => throw new NotSupportedException (),
        //};
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (null, _confirmation, Effects);

    public override string ToString (ref readonly SimulationContext context) {
        List<string> result = [StringLineFormatter.Indent (Name, 0)];
        throw new NotImplementedException ();
        

        // TODO: Filter and Confirmation

        //foreach (Effect e in Effects) {
        //    string filter = StringLineFormatter.Indent (DeclarerToString (e, in context), 1);
        //    result.Add (e.ToString (in context));
        //}

        //return string.Join ('\n', result);
    }
}
