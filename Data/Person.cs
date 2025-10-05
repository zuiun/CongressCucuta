namespace congress_cucuta.Data;

internal struct Person (IDType id, string name) : IID {
    public IDType ID => id;
    public string Name => name;
}
