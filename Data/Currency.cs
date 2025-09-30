namespace congress_cucuta.Data;

internal class Currency (byte id, string name, sbyte value) : IID {
    public byte ID { get; } = id;
    public string Name { get; } = name;
    public sbyte Value { get; set; } = value;

    public static Currency operator + (Currency left, Currency right) {
        return new Currency (left.ID, left.Name, (sbyte) (left.Value + right.Value));
    }

    public static Currency operator - (Currency left, Currency right) {
        return new Currency (left.ID, left.Name, (sbyte) (left.Value - right.Value));
    }

    public static implicit operator Currency (byte id) {
        return new Currency (id, "byte", 0);
    }

    public static implicit operator Currency (sbyte value) {
        return new Currency (0, "sbyte", value);
    }
}
