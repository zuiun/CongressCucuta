namespace congress_cucuta.Data;

internal interface IFaction : IID {
    string Name { get; }
    List<string> Description { get; }
    string? Leader { get; }
    Currency? Currency { get; }
}

internal readonly struct Party (
    byte id,
    string name,
    List<string> description,
    string leader,
    string? abbreviation = null,
    Currency? currency = null
) : IFaction {
    public byte ID { get; } = id;
    public string Name { get; } = name;
    public string? Abbreviation { get; } = abbreviation;
    public List<string> Description { get; } = description;
    public string Leader { get; } = leader;
    public Currency? Currency { get; } = currency;
}

internal readonly struct Region (
    byte id,
    string name,
    List<string> description,
    string leader,
    Currency? currency = null
) : IFaction {
    public byte ID { get; } = id;
    public string Name { get; } = name;
    public List<string> Description { get; } = description;
    public string Leader { get; } = leader;
    public Currency? Currency { get; } = currency;
}
