namespace CongressCucuta.Data;

internal readonly record struct IDType (byte ID) : IComparable<IDType>, IEquatable<byte> {
    public static implicit operator IDType (byte id) => new (id);

    public static implicit operator byte (IDType id) => id.ID;

    public static implicit operator IDType (int id) => new ((byte) id);

    public int CompareTo (IDType other) {
        if (ID < other.ID) {
            return -1;
        } else if (ID > other.ID) {
            return 1;
        } else {
            return 0;
        }
    }

    public bool Equals (byte other) => ID == other;

    public override string ToString () => $"{ID}";
}

internal interface IID {
    public IDType ID { get; }
}
