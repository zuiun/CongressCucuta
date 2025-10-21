using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct NeverCondition : ICondition {
    public bool Evaluate (SimulationContext context) => false;

    public string ToString (ref readonly Localisation localisation) => "End";

    public bool? YieldBallotVote () => null;
}
