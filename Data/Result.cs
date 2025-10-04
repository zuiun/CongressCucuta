namespace congress_cucuta.Data;

internal readonly struct Result (IDType id, string title, List<string> description, List<Link<Result>> links) : IID {
    public IDType ID => id;
    public string Title => title;
    public List<string> Description => description;
    public List<Link<Result>> Links => links;
}
