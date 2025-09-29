namespace congress_cucuta.Data; 

internal class Ballot (byte id) : IID {
    public byte ID { get; } = id;
    public bool IsIncident { get; } = false;
    public string Title { get; } = "Ballot A";
    public string Name { get; set; } = "Name";
    public List<string> Description { get; set; } = [];
    // TODO: results objects
    public List<Link<Ballot>> Links { get; set; } = [];
    public bool IsEnd { get; } = false;
}
