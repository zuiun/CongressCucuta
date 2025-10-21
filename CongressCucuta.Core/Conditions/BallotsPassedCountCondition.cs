using System.Diagnostics;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct BallotsPassedCountCondition (ComparisonType Comparison, byte Count) : ICondition {
    public bool Evaluate (SimulationContext context) {
        return Comparison switch {
            ComparisonType.Equal => context.GetBallotsPassedCount () == Count,
            ComparisonType.GreaterThan => context.GetBallotsPassedCount () > Count,
            ComparisonType.FewerThan => context.GetBallotsPassedCount () < Count,
            ComparisonType.GreaterThanOrEqual => context.GetBallotsPassedCount () >= Count,
            ComparisonType.FewerThanOrEqual => context.GetBallotsPassedCount () <= Count,
            _ => throw new UnreachableException (),
        };
    }

    public string ToString (ref readonly Localisation localisation) {
        return Comparison switch {
            ComparisonType.Equal => $"{Count} Ballot(s) Passed",
            ComparisonType.GreaterThan => $"Greater than {Count} Ballot(s) Passed",
            ComparisonType.FewerThan => $"Fewer than {Count} Ballot(s) Passed",
            ComparisonType.GreaterThanOrEqual => $"{Count} or Greater Ballot(s) Passed",
            ComparisonType.FewerThanOrEqual => $"{Count} or Fewer Ballot(s) Passed",
            _ => throw new UnreachableException (),
        };
    }

    public bool? YieldBallotVote () => null;
}
