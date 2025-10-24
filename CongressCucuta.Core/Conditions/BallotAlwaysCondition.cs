using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct BallotAlwaysCondition : ICondition {
    public bool Evaluate (SimulationContext context) => true;

    public string ToString (ref readonly Localisation localisation) => "Pass";

    public bool? YieldBallotVote () => true;
}
