namespace congress_cucuta.Data;

internal interface IFaction : IID {
    string Name { get; }
    List<string> Description { get; }
    string? Leader { get; }
    Currency? Currency { get; }
}
