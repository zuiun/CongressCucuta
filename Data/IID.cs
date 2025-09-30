namespace congress_cucuta.Data;

internal interface IID {
    public byte ID { get; }
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
