namespace congress_cucuta.Data;

internal readonly record struct IDType (byte ID) : IComparable<IDType> {
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
}

internal interface IID {
    public IDType ID { get; }
}

internal class IIDEqualityComparer : IEqualityComparer<IID> {
    public bool Equals (IID? left, IID? right) {
        if (left is null || right is null) {
            return false;
        }

        return left.ID == right.ID;
    }

    public int GetHashCode (IID id) => id.ID;
}
