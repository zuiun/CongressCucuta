using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

/*
 * Only intended for use during a Ballot vote to choose a Result
 * Do not use this in the creation of Links
 */
public readonly record struct BallotVoteCondition (bool ShouldBePassed) : ICondition {
    public bool Evaluate (SimulationContext context) {
        bool? result = context.IsBallotVoted ();

        return result == ShouldBePassed;
    }

    public string ToString (ref readonly Localisation localisation) => ShouldBePassed ? "Pass" : "Fail";

    public bool? YieldBallotVote () => ShouldBePassed;
}
