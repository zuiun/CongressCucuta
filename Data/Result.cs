namespace congress_cucuta.Data;

internal readonly struct Result (byte id, string title, List<string> description, List<Link<Result>> links) : IID {
    public byte ID { get; } = id;
    public string Title { get; } = title;
    public List<string> Description { get; } = description;
    public List<Link<Result>> Links { get; } = links;
}
