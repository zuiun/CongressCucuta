namespace congress_cucuta.Data;

internal class Currency (IDType id, string name) : IID {
    public static readonly IDType STATE = byte.MaxValue;
    public IDType ID { get; } = id;
    public string Name { get; } = name;

    //public static implicit operator Currency (IDType id) {
    //    return new Currency (id, nameof (IDType), 0);
    //}
}
