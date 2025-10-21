using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct BallotPassedCondition (IDType BallotID, bool ShouldBePassed = true) : ICondition {
    public bool Evaluate (SimulationContext context) => context.IsBallotPassed (BallotID) == ShouldBePassed;

    public string ToString (ref readonly Localisation localisation) {
        string title = localisation.Ballots[BallotID].Item1;

        return ShouldBePassed ? $"{title} Passed" : $"{title} Failed";
    }

    public bool? YieldBallotVote () => null;
}
