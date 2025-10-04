namespace congress_cucuta.Data;

internal class Faction : IID {
    public IDType ID { get; }
    public bool IsActiveStart { get; }

    public Faction (IDType id, bool isActiveStart = true) {
        if (id == Role.MEMBER || id == Role.HEAD_GOVERNMENT || id == Role.HEAD_STATE) {
            throw new ArgumentException ($"Faction ID {id} is reserved by Role", nameof (id));
        }

        ID = id;
        IsActiveStart = isActiveStart;
    }
}
