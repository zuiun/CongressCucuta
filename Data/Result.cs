namespace congress_cucuta.Data;

internal readonly struct Result (IDType id, string title, List<string> description, List<Link<Result>> links) : IID {
    public IDType ID { get; } = id;
    public string Title { get; } = title;
    public List<string> Description { get; } = description;
    public List<Link<Result>> Links { get; } = links;
}
