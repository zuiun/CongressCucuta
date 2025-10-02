namespace congress_cucuta.Data;

/*
 * Generic indicates which object's ID is referenced
 * It serves no functional purpose
 */
internal readonly struct Link<T> (Condition condition, IDType targetId)
where T : IID {
    private readonly Condition _condition = condition;
    public IDType TargetID { get; } = targetId;

    public bool? Evaluate (ref readonly SimulationContext context) => _condition.Evaluate (in context);

    public override string ToString () => _condition.ToString ();
}
