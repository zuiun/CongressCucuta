namespace congress_cucuta.Data;

/*
 * Generic indicates which object's ID is referenced
 * It serves no functional purpose
 */
internal readonly struct Link<IID> (ICondition condition, byte targetID) {
    public ICondition Condition { get; } = condition;
    public byte TargetID { get; } = targetID;

    public byte? Evaluate (ref readonly SimulationContext context) => Condition.Evaluate (context) ? TargetID : null;
}
