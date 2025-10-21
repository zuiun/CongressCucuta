namespace CongressCucuta.Data;

/*
 * T indicates which object's ID is referenced
 * It serves no functional purpose and exists only to prevent mixing of different Link<T>
 */
internal readonly record struct Link<T> (ICondition Condition, IDType TargetID)
where T : IID {
    public IDType? Resolve (SimulationContext context) => Condition.Evaluate (context) ? TargetID : null;
}
