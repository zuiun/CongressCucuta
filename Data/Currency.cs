namespace congress_cucuta.Data;

internal readonly struct Currency (IDType id, string name) : IID {
    public static readonly IDType STATE = byte.MaxValue;
    // These two reserved IDs exist for setup purposes and are converted into the appropriate Faction IDs
    // public static readonly IDType PARTY = STATE - 1;
    // public static readonly IDType REGION = PARTY - 1;

    public IDType ID => id;
    public string Name => name;
}
