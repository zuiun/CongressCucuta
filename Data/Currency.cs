namespace congress_cucuta.Data;

internal class Currency (string name, sbyte value) {
    string Name { get; } = name;
    sbyte Value { get; set; } = value;
}
