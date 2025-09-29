namespace congress_cucuta.Data;

internal interface ICondition {
    bool Evaluate (SimulationContext results);
}

public enum ValueComparison {
    Equal,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
}

internal class AndCondition (params ICondition[] conditions) : ICondition {
    private readonly ICondition[] conditions = conditions;

    public bool Evaluate (SimulationContext results) {
        foreach (var condition in conditions) {
            if (!condition.Evaluate (results)) {
                return false;
            }
        }

        return true;
    }
}

internal class OrCondition (params ICondition[] conditions) : ICondition {
    private readonly ICondition[] conditions = conditions;

    public bool Evaluate (SimulationContext results) {
        foreach (var condition in conditions) {
            if (condition.Evaluate (results)) {
                return true;
            }
        }

        return false;
    }
}

internal class BallotPassedCondition (byte ballotID, bool shouldBePassed) : ICondition {
    private readonly byte ballotID = ballotID;
    private readonly bool shouldBePassed = shouldBePassed;

    public bool Evaluate (SimulationContext results) {
        throw new NotImplementedException ();
    }
}

internal class BallotsPassedCondition (params byte[] ballotIDs) : ICondition {
    private readonly byte[] ballotIDs = ballotIDs;

    public bool Evaluate (SimulationContext results) {
        throw new NotImplementedException ();
    }
}

internal class BallotsPassedCountCondition (ValueComparison comparison, byte count) : ICondition {
    private readonly ValueComparison comparison = comparison;
    private readonly byte count = count;

    public bool Evaluate (SimulationContext results) {
        throw new NotImplementedException ();
    }
}

internal class CurrencyValueCondition (byte ownerID, ValueComparison comparison, byte value) : ICondition {
    private readonly byte ownerID = ownerID;
    private readonly ValueComparison comparison = comparison;
    private readonly byte value = value;

    public bool Evaluate (SimulationContext results) {
        throw new NotImplementedException ();
    }
}

internal class ProcedureActiveCondition (byte procedureID, bool shouldBeActive) : ICondition {
    private readonly byte procedureID = procedureID;
    private readonly bool shouldBeActive = shouldBeActive;

    public bool Evaluate (SimulationContext results) {
        throw new NotImplementedException ();
    }
}
