namespace congress_cucuta.Data;

internal class Person (byte id, string name) : IID {
    public byte ID { get; } = id;
    public string Name { get; } = name;
    public HashSet<byte> RoleIDs { get; } = [];

    public void AddRole (byte roleID) => RoleIDs.Add (roleID);

    public void RemoveRole (byte roleID) => RoleIDs.Remove (roleID);
}
