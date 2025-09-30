using System.Diagnostics;

namespace congress_cucuta.Data;

internal abstract class Procedure (
    byte id,
    string name,
    string description,
    Procedure.Effect[] effects,
    Procedure.ConfirmationType? confirmation = null,
    Procedure.TargetType? filter = null, // Ballots, Declarers
    params byte[] filterIDs // Ballots, Declarers
): IID {
    internal enum TargetType {
        Every,
        Except,
        Only,
    }

    // Presence indicates DeclaredProcedure
    internal enum ConfirmationType {
        Always,
        DivisionChamber,
        SingleDice,
        AdversarialDice,
    }

    internal readonly struct Effect (
        Effect.ActionType action,
        byte value,
        TargetType? target,
        byte[] targetIDs,
        TargetType? filter = null, // Allowed participants
        params byte[] filterIDs
    ) {
        internal enum ActionType {
            VotePassAdd, // Target: Ballot; Value: Added
            VoteFailAdd, // Target: Ballot; Value: Added
            VotePassTwoThirds, // Target: Ballot
            CurrencyAdd, // Target: Currency; Value: Added
            CurrencySubtract, // Target: Currency; Value: Subtracted
            CurrencySet, // Target: Currency; Value: Set
            ProcedureActivate, // Target: Procedure
            ElectionRandom, // Filter: Every; Role (excluded)
            ElectionNominated, // Target: Role (elected)
            Commitment,
            VotersLimit, // Target: None, Currency; Value: Compared (value), Compared (Currency)
            Appointment, // Target: Role
        }

        public ActionType Action { get; } = action;
        public byte Value { get; } = value;
        public TargetType? Target { get; } = target;
        public byte[] TargetIDs { get; } = targetIDs;
        public TargetType? Filter { get; } = filter;
        public byte[] FilterIDs { get; } = filterIDs;
    }

    internal readonly struct EffectBundle (byte? procedureID = null, ConfirmationType? confirmation = null, params Effect[] effects) {
        // Presence indicates ProcedureImmediate
        public byte? ProcedureID { get; } = procedureID;
        public Effect[] Effects { get; } = effects;
        public ConfirmationType? Confirmation { get; } = confirmation;
    }

    public byte ID { get; } = id;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public Effect[] Effects { get; } = effects;
    public ConfirmationType? Confirmation { get; } = confirmation;
    public TargetType? Filter { get; } = filter;
    public byte[] FilterIDs { get; } = filterIDs;

    public abstract EffectBundle? YieldEffects (ref readonly SimulationContext context);
}

/*
 * Activated once only at the beginning of a simulation
 * It is immediately deactivated after YieldEffect
 * It can only be activated again through a Procedure
 *
 * action: ElectionRandom, ElectionNominated
 */
internal class ProcedureImmediate : Procedure {
    public ProcedureImmediate (
        byte id,
        string name,
        string description,
        Effect[] effects
    ) : base (id, name, description, effects, 0) {
        foreach (var effect in effects) {
            if (effect.Action is not Effect.ActionType.ElectionRandom and not Effect.ActionType.ElectionNominated) {
                throw new ArgumentException ("action must be ElectionRandom or ElectionNominated");
            }
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (ID, Confirmation, Effects);
}

/*
 * Activates on targeted Ballots
 * filter controls on which ballots it activates
 * confirmationIDs only matters if filter is TargetType.Only or TargetType.Except
 *
 * action: != ElectionRandom, != ElectionNominated, != Commitment, != VotersLimit, != Appointment
 * filter: Ballot
 */
internal class ProcedureTargeted : Procedure {
    public ProcedureTargeted (
        byte id,
        string name,
        string description,
        Effect[] effects,
        TargetType filter,
        params byte[] filterIDs
    ) : base (id, name, description, effects, null, filter, filterIDs) {
        foreach (var effect in effects) {
            if (
                effect.Action is Effect.ActionType.ElectionRandom
                or Effect.ActionType.ElectionNominated
                or Effect.ActionType.Commitment
                or Effect.ActionType.VotersLimit
                or Effect.ActionType.Appointment
            ) {
                throw new ArgumentException ("action cannot be ElectionRandom, ElectionNominated, Commitment, VotersLimit, or Appointment");
            }
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) {
        bool isBallotMatched = FilterIDs.Contains (context.BallotCurrent);

        return Filter switch {
            TargetType.Every => new (null, Confirmation, Effects),
            TargetType.Except => isBallotMatched ? null : new (null, Confirmation, Effects),
            TargetType.Only => isBallotMatched ? new (null, Confirmation, Effects) : null,
            _ => throw new UnreachableException (),
        };
    }
}

/*
 * Activates declaratively
 * Empty confirmationIDs means it activates on every Ballot
 *
 * action: CurrencyAdd, CurrencySubtract, CurrencySet, ElectionRandom, Commitment, VotersLimit, Appointment
 * filter: Role (declarer)
 */
internal class ProcedureDeclared : Procedure {
    public ProcedureDeclared (
        byte id,
        string name,
        string description,
        Effect[] effects,
        ConfirmationType? confirmation,
        TargetType filter,
        params byte[] filterIDs
    ) : base (id, name, description, effects, confirmation, filter, filterIDs) {
        foreach (var effect in effects) {
            if (
                effect.Action is not Effect.ActionType.CurrencyAdd
                and not Effect.ActionType.CurrencySubtract
                and not Effect.ActionType.CurrencySet
                and not Effect.ActionType.ElectionRandom
                and not Effect.ActionType.Commitment
                and not Effect.ActionType.VotersLimit
                and not Effect.ActionType.Appointment
            ) {
                throw new ArgumentException ("action must be CurrencyAdd, CurrencySubtract, CurrencySet, ElectionRandom, Commitment, VotersLimit, or Appointment");
            }
        }
    }

    public override EffectBundle? YieldEffects (ref readonly SimulationContext context) => new (null, Confirmation, Effects);
}
