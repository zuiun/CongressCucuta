namespace congress_cucuta.Data;

/*
 * T is constrained to signal intent and remind the user which object's ID is being referenced
 * It serves no functional purpose
 */
internal struct Link<T> (ICondition condition, byte targetID) where T: IID {
    public ICondition Condition { get; set; } = condition;
    public byte TargetID { get; set; } = targetID;
}
