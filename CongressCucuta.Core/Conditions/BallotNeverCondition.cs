using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct BallotNeverCondition : ICondition {
    public bool Evaluate (SimulationContext context) => true;

    public string ToString (ref readonly Localisation localisation) => "Fail";

    public bool? YieldBallotVote () => false;
}
