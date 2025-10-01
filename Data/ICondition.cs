using System.Diagnostics;

namespace congress_cucuta.Data;

internal interface ICondition {
    internal enum ComparisonType {
        Equal,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
    }

    public bool Evaluate (SimulationContext context);
}

internal readonly struct AlwaysCondition () : ICondition {
    public bool Evaluate (SimulationContext context) => true;
}

internal readonly struct AndCondition (params ICondition[] conditions) : ICondition {
    public ICondition[] Conditions { get; } = conditions;

    public bool Evaluate (SimulationContext context) {
        foreach (var condition in Conditions) {
            if (! condition.Evaluate (context)) {
                return false;
            }
        }

        return true;
    }
}

internal readonly struct OrCondition (params ICondition[] conditions) : ICondition {
    public ICondition[] Conditions { get; } = conditions;

    public bool Evaluate (SimulationContext context) {
        foreach (var condition in Conditions) {
            if (condition.Evaluate (context)) {
                return true;
            }
        }

        return false;
    }
}

internal readonly struct BallotPassedCondition (IDType ballotID, bool shouldBePassed = true) : ICondition {
    public IDType BallotID { get; } = ballotID;
    public bool ShouldBePassed { get; } = shouldBePassed;

    public bool Evaluate (SimulationContext context) {
        return context.IsBallotPassed (BallotID) == ShouldBePassed;
    }
}

internal readonly struct BallotsPassedCountCondition (ICondition.ComparisonType comparison, IDType count) : ICondition {
    public ICondition.ComparisonType Comparison { get; } = comparison;
    public IDType Count { get; } = count;

    public bool Evaluate (SimulationContext context) {
        return Comparison switch {
            ICondition.ComparisonType.Equal => context.GetBallotsPassedCount () == Count,
            ICondition.ComparisonType.GreaterThan => context.GetBallotsPassedCount () > Count,
            ICondition.ComparisonType.LessThan => context.GetBallotsPassedCount () < Count,
            ICondition.ComparisonType.GreaterThanOrEqual => context.GetBallotsPassedCount () >= Count,
            ICondition.ComparisonType.LessThanOrEqual => context.GetBallotsPassedCount () <= Count,
            _ => throw new UnreachableException (),
        };
    }
}

internal readonly struct CurrencyValueCondition (IDType ownerID, ICondition.ComparisonType comparison, byte value) : ICondition {
    public IDType OwnerID { get; } = ownerID;
    public ICondition.ComparisonType Comparison { get; } = comparison;
    public byte Value { get; } = value;

    public bool Evaluate (SimulationContext context) {
        return Comparison switch {
            ICondition.ComparisonType.Equal => context.GetCurrencyValue (OwnerID) == Value,
            ICondition.ComparisonType.GreaterThan => context.GetCurrencyValue (OwnerID) > Value,
            ICondition.ComparisonType.LessThan => context.GetCurrencyValue (OwnerID) < Value,
            ICondition.ComparisonType.GreaterThanOrEqual => context.GetCurrencyValue (OwnerID) >= Value,
            ICondition.ComparisonType.LessThanOrEqual => context.GetCurrencyValue (OwnerID) <= Value,
            _ => throw new UnreachableException (),
        };
    }
}

internal readonly struct ProcedureActiveCondition (IDType procedureID, bool shouldBeActive) : ICondition {
    public IDType ProcedureID { get; } = procedureID;
    public bool ShouldBeActive { get; } = shouldBeActive;

    public bool Evaluate (SimulationContext context) {
        return context.IsProcedureActive (ProcedureID) == ShouldBeActive;
    }
}
