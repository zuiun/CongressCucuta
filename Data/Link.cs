namespace congress_cucuta.Data;

/*
 * T indicates which object's ID is referenced
 * It serves no functional purpose and exists only to prevent mixing of different Link<T>
 */
internal readonly struct Link<T> (Condition condition, IDType targetId)
where T : IID {
    public IDType TargetID { get; } = targetId;

    public bool? Evaluate (ref readonly SimulationContext context) => condition.Evaluate (in context);

    public override string ToString () => condition.ToString ();
}
