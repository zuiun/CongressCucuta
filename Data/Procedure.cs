namespace congress_cucuta.Data;

internal abstract class Procedure (byte id) : IID {
    internal readonly struct Effect {
        internal enum ActionType {
            VotePassAdd,
            VoteFailAdd,
            VotePassTwoThirds,
            CurrencyAdd,
            CurrencySubtract,
            CurrencySet,
            ProcedureActivate, // Only intended for elections (subset of ProcedureImmediate)
            PermissionsChange,
            // TODO: how on earth are most declared procedures supposed to work?
            // todo we have all sorts of types of rebellions, civil wars, coups, joint coups, dissolutions, motions, commitment of responsibility, refusal of supply, etc
        }
    }

    internal enum TargetType {
        Everyone,
        EveryoneExcept,
        Specific,
    }

    public byte ID { get; } = id;

    public abstract Procedure.Effect YieldEffect (SimulationContext context);
}

internal class ProcedureImmediate (byte id) : Procedure (id) {
    public override Procedure.Effect YieldEffect (SimulationContext context) {
        throw new NotImplementedException ();
    }
}

internal class ProcedureRepeating (byte id) : Procedure (id) {
    public override Procedure.Effect YieldEffect (SimulationContext context) {
        throw new NotImplementedException ();
    }
}

internal class ProcedureTargeted (byte id) : Procedure (id) {
    public override Procedure.Effect YieldEffect (SimulationContext context) {
        throw new NotImplementedException ();
    }
}

internal class ProcedureDeclared (byte id) : Procedure (id) {
    public override Procedure.Effect YieldEffect (SimulationContext context) {
        throw new NotImplementedException ();
    }
}
