namespace congress_cucuta.Data;

internal class Currency (IDType id, string name, sbyte value) : IID {
    public const byte STATE = byte.MaxValue;

    public IDType ID { get; } = id;
    public string Name { get; } = name;
    public sbyte Value { get; set; } = value;

    public static Currency operator + (Currency left, Currency right) {
        return new Currency (left.ID, left.Name, (sbyte) (left.Value + right.Value));
    }

    public static Currency operator - (Currency left, Currency right) {
        return new Currency (left.ID, left.Name, (sbyte) (left.Value - right.Value));
    }

    public static implicit operator Currency (IDType id) {
        return new Currency (id, nameof (IDType), 0);
    }
}
