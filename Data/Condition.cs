using System.Diagnostics;

namespace congress_cucuta.Data;

internal abstract class Condition {
    internal enum ComparisonType {
        Equal,
        GreaterThan,
        FewerThan,
        GreaterThanOrEqual,
        FewerThanOrEqual,
    }

    public abstract bool Evaluate (ref readonly SimulationContext context);

    public abstract string ToString (ref readonly Localisation localisation);
}

internal class AlwaysCondition : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) => true;

    public override string ToString (ref readonly Localisation localisation) => "Next";
}

internal class NeverCondition : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) => false;

    public override string ToString (ref readonly Localisation localisation) => "End";
}

internal class AndCondition (params Condition[] conditions) : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) {
        foreach (Condition condition in conditions) {
            if (! condition.Evaluate (in context)) {
                return false;
            }
        }

        return true;
    }

    public override string ToString (ref readonly Localisation localisation) => string.Join (" and ", conditions.Select (c => c.ToString ()));
}

internal class OrCondition (params Condition[] conditions) : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) {
        foreach (Condition condition in conditions) {
            if (condition.Evaluate (in context)) {
                return true;
            }
        }

        return false;
    }

    public override string ToString (ref readonly Localisation localisation) => string.Join (" or ", conditions.Select (c => c.ToString ()));
}

/*
 * Only intended for use during a Ballot vote to choose a Result
 * Do not use this in the creation of Links
 */
internal class BallotVoteCondition (bool shouldBePassed) : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) {
        bool? result = context.IsBallotVoted ();

        return result == shouldBePassed;
    }

    public override string ToString (ref readonly Localisation localisation) => shouldBePassed ? "Pass" : "Fail";
}

internal class BallotPassedCondition (ref readonly Ballot ballot, bool shouldBePassed = true) : Condition {
    private readonly IDType _ballotId = ballot.ID;

    public override bool Evaluate (ref readonly SimulationContext context) => context.IsBallotPassed (_ballotId) == shouldBePassed;

    public override string ToString (ref readonly Localisation localisation) {
        string title = localisation.Ballots[_ballotId].Item1;

        return shouldBePassed ? $"{title} Passed" : $"{title} Failed";
    }
}

internal class BallotsPassedCountCondition (Condition.ComparisonType comparison, byte count) : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) {
        return comparison switch {
            ComparisonType.Equal => context.GetBallotsPassedCount () == count,
            ComparisonType.GreaterThan => context.GetBallotsPassedCount () > count,
            ComparisonType.FewerThan => context.GetBallotsPassedCount () < count,
            ComparisonType.GreaterThanOrEqual => context.GetBallotsPassedCount () >= count,
            ComparisonType.FewerThanOrEqual => context.GetBallotsPassedCount () <= count,
            _ => throw new UnreachableException (),
        };
    }

    public override string ToString (ref readonly Localisation localisation) {
        return comparison switch {
            ComparisonType.Equal => $"{count} Ballots Passed",
            ComparisonType.GreaterThan => $"Greater than {count} Ballots Passed",
            ComparisonType.FewerThan => $"Fewer than {count} Ballots Passed",
            ComparisonType.GreaterThanOrEqual => $"{count} or Greater Ballots Passed",
            ComparisonType.FewerThanOrEqual => $"{count} or Fewer Ballots Passed",
            _ => throw new UnreachableException (),
        };
    }
}

internal class CurrencyValueCondition (ref readonly Currency currency, Condition.ComparisonType comparison, byte value) : Condition {
    private readonly IDType _currencyId = currency.ID;

    public override bool Evaluate (ref readonly SimulationContext context) {
        return comparison switch {
            ComparisonType.Equal => context.GetCurrencyValue (_currencyId) == value,
            ComparisonType.GreaterThan => context.GetCurrencyValue (_currencyId) > value,
            ComparisonType.FewerThan => context.GetCurrencyValue (_currencyId) < value,
            ComparisonType.GreaterThanOrEqual => context.GetCurrencyValue (_currencyId) >= value,
            ComparisonType.FewerThanOrEqual => context.GetCurrencyValue (_currencyId) <= value,
            _ => throw new UnreachableException (),
        };
    }

    public override string ToString (ref readonly Localisation localisation) {
        string name = localisation.Currencies[_currencyId];

        return comparison switch {
            ComparisonType.Equal => $"{value} {name}",
            ComparisonType.GreaterThan => $"Greater than {value} {name}",
            ComparisonType.FewerThan => $"Fewer than {value} {name}",
            ComparisonType.GreaterThanOrEqual => $"{value} or Greater {name}",
            ComparisonType.FewerThanOrEqual => $"{value} or Fewer {name}",
            _ => throw new UnreachableException (),
        };
    }
}

internal class ProcedureActiveCondition (ref readonly ProcedureTargeted procedure, bool shouldBeActive) : Condition {
    private readonly IDType _procedureId = procedure.ID;

    public override bool Evaluate (ref readonly SimulationContext context) => context.IsProcedureActive (_procedureId) == shouldBeActive;
    
    public override string ToString (ref readonly Localisation localisation) {
        string name = localisation.Procedures[_procedureId].Item1;

        return shouldBeActive ? $"{name} is Active" : $"{name} is not Active";
    }
}
