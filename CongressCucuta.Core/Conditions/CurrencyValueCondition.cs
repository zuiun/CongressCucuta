using System.Diagnostics;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct CurrencyValueCondition (IDType CurrencyID, ComparisonType Comparison, sbyte Value) : ICondition {
    public bool Evaluate (SimulationContext context) {
        return Comparison switch {
            ComparisonType.Equal => context.GetCurrencyValue (CurrencyID) == Value,
            ComparisonType.GreaterThan => context.GetCurrencyValue (CurrencyID) > Value,
            ComparisonType.FewerThan => context.GetCurrencyValue (CurrencyID) < Value,
            ComparisonType.GreaterThanOrEqual => context.GetCurrencyValue (CurrencyID) >= Value,
            ComparisonType.FewerThanOrEqual => context.GetCurrencyValue (CurrencyID) <= Value,
            _ => throw new UnreachableException (),
        };
    }

    public string ToString (ref readonly Localisation localisation) {
        string name = localisation.Currencies[CurrencyID];

        return Comparison switch {
            ComparisonType.Equal => $"{Value} {name}",
            ComparisonType.GreaterThan => $"Greater than {Value} {name}",
            ComparisonType.FewerThan => $"Fewer than {Value} {name}",
            ComparisonType.GreaterThanOrEqual => $"{Value} or Greater {name}",
            ComparisonType.FewerThanOrEqual => $"{Value} or Fewer {name}",
            _ => throw new UnreachableException (),
        };
    }

    public bool? YieldBallotVote () => null;
}
