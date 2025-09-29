
namespace congress_cucuta.Data;

internal class Region (byte id) : IFaction {
    public byte ID { get; } = id;
    public string Name => throw new NotImplementedException ();
    public List<string> Description => throw new NotImplementedException ();
    public string? Leader => throw new NotImplementedException ();
    public Currency? Currency => throw new NotImplementedException ();
}
