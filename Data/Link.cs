namespace congress_cucuta.Data;

/*
 * Generic indicates which object's ID is referenced
 * It serves no functional purpose
 */
internal readonly struct Link<IID> (ICondition condition, IDType targetID) {
    public ICondition Condition { get; } = condition;
    public IDType TargetID { get; } = targetID;

    public IDType? Evaluate (ref readonly SimulationContext context) => Condition.Evaluate (in context) ? TargetID : null;
}
