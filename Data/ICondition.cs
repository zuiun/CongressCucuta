using System.Diagnostics;

namespace congress_cucuta.Data;

internal enum Comparison {
    Equal,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
}

internal interface ICondition {
    public bool Evaluate (SimulationContext context);
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

internal readonly struct BallotPassedCondition (byte ballotID, bool shouldBePassed = true) : ICondition {
    public byte BallotID { get; } = ballotID;
    public bool ShouldBePassed { get; } = shouldBePassed;

    public bool Evaluate (SimulationContext context) {
        return context.IsBallotPassed (BallotID) == ShouldBePassed;
    }
}

internal readonly struct BallotsPassedCountCondition (Comparison comparison, byte count) : ICondition {
    public Comparison Comparison { get; } = comparison;
    public byte Count { get; } = count;

    public bool Evaluate (SimulationContext context) {
        return Comparison switch {
            Comparison.Equal => context.GetBallotsPassedCount () == Count,
            Comparison.GreaterThan => context.GetBallotsPassedCount () > Count,
            Comparison.LessThan => context.GetBallotsPassedCount () < Count,
            Comparison.GreaterThanOrEqual => context.GetBallotsPassedCount () >= Count,
            Comparison.LessThanOrEqual => context.GetBallotsPassedCount () <= Count,
            _ => throw new UnreachableException (),
        };
    }
}

internal readonly struct CurrencyValueCondition (byte ownerID, Comparison comparison, byte value) : ICondition {
    public byte OwnerID { get; } = ownerID;
    public Comparison Comparison { get; } = comparison;
    public byte Value { get; } = value;

    public bool Evaluate (SimulationContext context) {
        return Comparison switch {
            Comparison.Equal => context.GetCurrencyValue (OwnerID) == Value,
            Comparison.GreaterThan => context.GetCurrencyValue (OwnerID) > Value,
            Comparison.LessThan => context.GetCurrencyValue (OwnerID) < Value,
            Comparison.GreaterThanOrEqual => context.GetCurrencyValue (OwnerID) >= Value,
            Comparison.LessThanOrEqual => context.GetCurrencyValue (OwnerID) <= Value,
            _ => throw new UnreachableException (),
        };
    }
}

internal readonly struct ProcedureActiveCondition (byte procedureID, bool shouldBeActive) : ICondition {
    public byte ProcedureID { get; } = procedureID;
    public bool ShouldBeActive { get; } = shouldBeActive;

    public bool Evaluate (SimulationContext context) {
        return context.IsProcedureActive (ProcedureID) == ShouldBeActive;
    }
}
