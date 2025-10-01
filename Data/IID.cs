namespace congress_cucuta.Data;

internal record IDType (byte ID) {
    public static implicit operator IDType (byte id) => new (id);
    public static implicit operator byte (IDType id) => id.ID;
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
