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

    public abstract override string ToString ();
}

internal class AlwaysCondition : Condition {
    public override bool Evaluate (ref readonly SimulationContext context) => true;

    public override string ToString () => "Next";
}

internal class AndCondition (params Condition[] conditions) : Condition {
    private readonly Condition[] _conditions = conditions;

    public override bool Evaluate (ref readonly SimulationContext context) {
        foreach (Condition condition in _conditions) {
            if (! condition.Evaluate (in context)) {
                return false;
            }
        }

        return true;
    }

    public override string ToString () => string.Join (" and ", _conditions.Select (c => c.ToString ()));
}

internal class OrCondition (params Condition[] conditions) : Condition {
    private readonly Condition[] _conditions = conditions;

    public override bool Evaluate (ref readonly SimulationContext context) {
        foreach (Condition condition in _conditions) {
            if (condition.Evaluate (in context)) {
                return true;
            }
        }

        return false;
    }

    public override string ToString () => string.Join (" or ", _conditions.Select (c => c.ToString ()));
}

/*
 * Only intended for use during a Ballot vote to choose a Result
 * Do not use this in the creation of Links
 */
internal class BallotVoteCondition (bool shouldBePassed = true) : Condition {
    private readonly bool _shouldBePassed = shouldBePassed;

    public override bool Evaluate (ref readonly SimulationContext context) {
        bool? result = context.IsBallotVoted ();

        return result == _shouldBePassed;
    }

    public override string ToString () => _shouldBePassed ? "Pass" : "Fail";
}

internal class BallotPassedCondition (ref readonly Ballot ballot, bool shouldBePassed = true) : Condition {
    private readonly IDType _ballotId = ballot.ID;
    private readonly string _ballotTitle = ballot.Title;
    private readonly bool _shouldBePassed = shouldBePassed;

    public override bool Evaluate (ref readonly SimulationContext context) => context.IsBallotPassed (_ballotId) == _shouldBePassed;

    public override string ToString () => _shouldBePassed ? $"{_ballotTitle} Passed" : $"{_ballotTitle} Failed";
}

internal class BallotsPassedCountCondition (Condition.ComparisonType comparison, byte count) : Condition {
    private readonly ComparisonType _comparison = comparison;
    private readonly byte _count = count;

    public override bool Evaluate (ref readonly SimulationContext context) {
        return _comparison switch {
            ComparisonType.Equal => context.GetBallotsPassedCount () == _count,
            ComparisonType.GreaterThan => context.GetBallotsPassedCount () > _count,
            ComparisonType.FewerThan => context.GetBallotsPassedCount () < _count,
            ComparisonType.GreaterThanOrEqual => context.GetBallotsPassedCount () >= _count,
            ComparisonType.FewerThanOrEqual => context.GetBallotsPassedCount () <= _count,
            _ => throw new UnreachableException (),
        };
    }

    public override string ToString () {
        return _comparison switch {
            ComparisonType.Equal => $"{_count} Ballots Passed",
            ComparisonType.GreaterThan => $"Greater than {_count} Ballots Passed",
            ComparisonType.FewerThan => $"Fewer than {_count} Ballots Passed",
            ComparisonType.GreaterThanOrEqual => $"{_count} or Greater Ballots Passed",
            ComparisonType.FewerThanOrEqual => $"{_count} or Fewer Ballots Passed",
            _ => throw new UnreachableException (),
        };
    }
}

internal class CurrencyValueCondition (ref readonly Currency currency, Condition.ComparisonType comparison, byte value) : Condition {
    private readonly IDType _currencyId = currency.ID;
    private readonly string _currencyName = currency.Name;
    private readonly ComparisonType _comparison = comparison;
    private readonly byte _value = value;

    public override bool Evaluate (ref readonly SimulationContext context) {
        return _comparison switch {
            ComparisonType.Equal => context.GetCurrencyValue (_currencyId) == _value,
            ComparisonType.GreaterThan => context.GetCurrencyValue (_currencyId) > _value,
            ComparisonType.FewerThan => context.GetCurrencyValue (_currencyId) < _value,
            ComparisonType.GreaterThanOrEqual => context.GetCurrencyValue (_currencyId) >= _value,
            ComparisonType.FewerThanOrEqual => context.GetCurrencyValue (_currencyId) <= _value,
            _ => throw new UnreachableException (),
        };
    }

    public override string ToString () {
        return _comparison switch {
            ComparisonType.Equal => $"{_value} {_currencyName}",
            ComparisonType.GreaterThan => $"Greater than {_value} {_currencyName}",
            ComparisonType.FewerThan => $"Fewer than {_value} {_currencyName}",
            ComparisonType.GreaterThanOrEqual => $"{_value} or Greater {_currencyName}",
            ComparisonType.FewerThanOrEqual => $"{_value} or Fewer {_currencyName}",
            _ => throw new UnreachableException (),
        };
    }
}

internal class ProcedureActiveCondition (ref readonly Procedure procedure, bool shouldBeActive) : Condition {
    private readonly IDType _procedureId = procedure.ID;
    private readonly string _procedureName = procedure.Name;
    private readonly bool _shouldBeActive = shouldBeActive;

    public override bool Evaluate (ref readonly SimulationContext context) => context.IsProcedureActive (_procedureId) == _shouldBeActive;
    
    public override string ToString () => _shouldBeActive ? $"{_procedureName} is Active" : $"{_procedureName} is not Active";
}
