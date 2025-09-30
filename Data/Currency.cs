namespace congress_cucuta.Data;

internal struct Currency (string name, sbyte value) {
    string Name { get; } = name;
    sbyte Value { get; set; } = value;

    public static Currency operator + (Currency left, Currency right) {
        return new Currency (left.Name, (sbyte) (left.Value + right.Value));
    }

    public static Currency operator - (Currency left, Currency right) {
        return new Currency (left.Name, (sbyte) (left.Value - right.Value));
    }

    public static implicit operator Currency (sbyte value) {
        return new Currency ("sbyte", value);
    }
}
