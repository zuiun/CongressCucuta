using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct ProcedureActiveCondition (IDType ProcedureID, bool ShouldBeActive) : ICondition {
    public bool Evaluate (SimulationContext context) => context.IsProcedureActive (ProcedureID) == ShouldBeActive;

    public string ToString (ref readonly Localisation localisation) {
        string name = localisation.Procedures[ProcedureID].Item1;

        return ShouldBeActive ? $"{name} is Active" : $"{name} is not Active";
    }

    public bool? YieldBallotVote () => null;
}
