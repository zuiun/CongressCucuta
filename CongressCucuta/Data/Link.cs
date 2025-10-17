namespace CongressCucuta.Data;

/*
 * T indicates which object's ID is referenced
 * It serves no functional purpose and exists only to prevent mixing of different Link<T>
 */
internal readonly record struct Link<T> (ICondition Condition, IDType TargetID)
where T : IID {
    public bool Evaluate (ref readonly SimulationContext context) => Condition.Evaluate (in context);

    public string ToString (ref readonly Localisation localisation) => Condition.ToString (in localisation);

    public bool? YieldBallotVote () => Condition.YieldBallotVote ();
}
