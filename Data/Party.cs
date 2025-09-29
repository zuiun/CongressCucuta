namespace congress_cucuta.Data;

internal class Party (byte id) : IFaction {
    public byte ID { get; } = id;
    public string Name => throw new NotImplementedException ();
    public string? Abbreviation => throw new NotImplementedException ();
    public List<string> Description => throw new NotImplementedException ();
    public string Leader => throw new NotImplementedException ();
    public Currency? Currency => throw new NotImplementedException ();
}
