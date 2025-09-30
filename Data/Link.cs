namespace congress_cucuta.Data;

/*
 * T is constrained to indicate which object's ID is referenced
 * It serves no functional purpose
 */
internal readonly struct Link<T> (ICondition condition, byte targetID) where T: IID {
    public ICondition Condition { get; } = condition;
    public byte TargetID { get; } = targetID;
}
