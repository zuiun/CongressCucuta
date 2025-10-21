using System.Text.Json.Serialization;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

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
public interface ICondition {
    

    public bool Evaluate (SimulationContext context);
    public string ToString (ref readonly Localisation localisation);
    public bool? YieldBallotVote ();
}
