namespace congress_cucuta.Data;

internal class Person (IDType id, string name) : IID {
    public IDType ID { get; } = id;
    public string Name { get; } = name;
    public HashSet<IDType> RoleIDs { get; } = [];

    public void AddRole (IDType roleId) => RoleIDs.Add (roleId);

    public void RemoveRole (IDType roleId) => RoleIDs.Remove (roleId);
}
