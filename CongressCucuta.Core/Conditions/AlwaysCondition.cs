using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct AlwaysCondition : ICondition {
    public bool Evaluate (SimulationContext context) => true;

    public string ToString (ref readonly Localisation localisation) => "Next";

    public bool? YieldBallotVote () => null;
}
