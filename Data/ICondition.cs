using System.Diagnostics;
using System.Text.Json.Serialization;

namespace congress_cucuta.Data;

[JsonPolymorphic (TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType (typeof (AlwaysCondition), nameof (AlwaysCondition))]
[JsonDerivedType (typeof (NeverCondition), nameof (NeverCondition))]
[JsonDerivedType (typeof (AndCondition), nameof (AndCondition))]
[JsonDerivedType (typeof (OrCondition), nameof (OrCondition))]
[JsonDerivedType (typeof (BallotVoteCondition), nameof (BallotVoteCondition))]
[JsonDerivedType (typeof (BallotPassedCondition), nameof (BallotPassedCondition))]
[JsonDerivedType (typeof (BallotsPassedCountCondition), nameof (BallotsPassedCountCondition))]
[JsonDerivedType (typeof (CurrencyValueCondition), nameof (CurrencyValueCondition))]
[JsonDerivedType (typeof (ProcedureActiveCondition), nameof (ProcedureActiveCondition))]
internal interface ICondition {
    internal enum ComparisonType {
        Equal,
        GreaterThan,
        FewerThan,
        GreaterThanOrEqual,
        FewerThanOrEqual,
    }

    public bool Evaluate (ref readonly SimulationContext context);
    public string ToString (ref readonly Localisation localisation);
    public bool? YieldBallotVote ();
}

internal readonly record struct AlwaysCondition : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) => true;

    public string ToString (ref readonly Localisation localisation) => "Next";

    public bool? YieldBallotVote () => null;
}

internal readonly record struct NeverCondition : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) => false;

    public string ToString (ref readonly Localisation localisation) => "End";

    public bool? YieldBallotVote () => null;
}

internal readonly record struct AndCondition (params ICondition[] Conditions) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) {
        foreach (ICondition c in Conditions) {
            if (! c.Evaluate (in context)) {
                return false;
            }
        }

        return true;
    }

    public string ToString (ref readonly Localisation localisation) {
        List<string> conditions = [];

        foreach (ICondition c in Conditions) {
            conditions.Add (c.ToString (in localisation));
        }

        return string.Join (" and ", conditions);
    }

    public bool? YieldBallotVote () => null;
}

internal readonly record struct OrCondition (params ICondition[] Conditions) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) {
        foreach (ICondition c in Conditions) {
            if (c.Evaluate (in context)) {
                return true;
            }
        }

        return false;
    }

    public string ToString (ref readonly Localisation localisation) {
        List<string> conditions = [];

        foreach (ICondition c in Conditions) {
            conditions.Add (c.ToString (in localisation));
        }

        return string.Join (" or ", conditions);
    }

    public bool? YieldBallotVote () => null;
}

/*
 * Only intended for use during a Ballot vote to choose a Result
 * Do not use this in the creation of Links
 */
internal readonly record struct BallotVoteCondition (bool ShouldBePassed) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) {
        bool? result = context.IsBallotVoted ();

        return result == ShouldBePassed;
    }

    public string ToString (ref readonly Localisation localisation) => ShouldBePassed ? "Pass" : "Fail";

    public bool? YieldBallotVote () => ShouldBePassed;
}

internal readonly record struct BallotPassedCondition (IDType BallotID, bool ShouldBePassed = true) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) => context.IsBallotPassed (BallotID) == ShouldBePassed;

    public string ToString (ref readonly Localisation localisation) {
        string title = localisation.Ballots[BallotID].Item1;

        return ShouldBePassed ? $"{title} Passed" : $"{title} Failed";
    }

    public bool? YieldBallotVote () => null;
}

internal readonly record struct BallotsPassedCountCondition (ICondition.ComparisonType Comparison, byte Count) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) {
        return Comparison switch {
            ICondition.ComparisonType.Equal => context.GetBallotsPassedCount () == Count,
            ICondition.ComparisonType.GreaterThan => context.GetBallotsPassedCount () > Count,
            ICondition.ComparisonType.FewerThan => context.GetBallotsPassedCount () < Count,
            ICondition.ComparisonType.GreaterThanOrEqual => context.GetBallotsPassedCount () >= Count,
            ICondition.ComparisonType.FewerThanOrEqual => context.GetBallotsPassedCount () <= Count,
            _ => throw new UnreachableException (),
        };
    }

    public string ToString (ref readonly Localisation localisation) {
        return Comparison switch {
            ICondition.ComparisonType.Equal => $"{Count} Ballots Passed",
            ICondition.ComparisonType.GreaterThan => $"Greater than {Count} Ballots Passed",
            ICondition.ComparisonType.FewerThan => $"Fewer than {Count} Ballots Passed",
            ICondition.ComparisonType.GreaterThanOrEqual => $"{Count} or Greater Ballots Passed",
            ICondition.ComparisonType.FewerThanOrEqual => $"{Count} or Fewer Ballots Passed",
            _ => throw new UnreachableException (),
        };
    }

    public bool? YieldBallotVote () => null;
}

internal readonly record struct CurrencyValueCondition (IDType CurrencyID, ICondition.ComparisonType Comparison, byte Value) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) {
        return Comparison switch {
            ICondition.ComparisonType.Equal => context.GetCurrencyValue (CurrencyID) == Value,
            ICondition.ComparisonType.GreaterThan => context.GetCurrencyValue (CurrencyID) > Value,
            ICondition.ComparisonType.FewerThan => context.GetCurrencyValue (CurrencyID) < Value,
            ICondition.ComparisonType.GreaterThanOrEqual => context.GetCurrencyValue (CurrencyID) >= Value,
            ICondition.ComparisonType.FewerThanOrEqual => context.GetCurrencyValue (CurrencyID) <= Value,
            _ => throw new UnreachableException (),
        };
    }

    public string ToString (ref readonly Localisation localisation) {
        string name = localisation.Currencies[CurrencyID];

        return Comparison switch {
            ICondition.ComparisonType.Equal => $"{Value} {name}",
            ICondition.ComparisonType.GreaterThan => $"Greater than {Value} {name}",
            ICondition.ComparisonType.FewerThan => $"Fewer than {Value} {name}",
            ICondition.ComparisonType.GreaterThanOrEqual => $"{Value} or Greater {name}",
            ICondition.ComparisonType.FewerThanOrEqual => $"{Value} or Fewer {name}",
            _ => throw new UnreachableException (),
        };
    }

    public bool? YieldBallotVote () => null;
}

internal readonly record struct ProcedureActiveCondition (IDType ProcedureID, bool ShouldBeActive) : ICondition {
    public bool Evaluate (ref readonly SimulationContext context) => context.IsProcedureActive (ProcedureID) == ShouldBeActive;
    
    public string ToString (ref readonly Localisation localisation) {
        string name = localisation.Procedures[ProcedureID].Item1;

        return ShouldBeActive ? $"{name} is Active" : $"{name} is not Active";
    }

    public bool? YieldBallotVote () => null;
}
