namespace congress_cucuta.Data;

internal readonly struct Result (IDType id, List<Link<Result>> links) : IID {
    public IDType ID => id;
    public List<Link<Result>> Links => links;
}
