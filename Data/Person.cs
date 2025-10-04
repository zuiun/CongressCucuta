namespace congress_cucuta.Data;

internal class Person (IDType id, string name) : IID {
    public IDType ID => id;
    public string Name => name;
    public HashSet<IDType> RoleIDs => [];

    public void AddRole (IDType roleId) => RoleIDs.Add (roleId);

    public void RemoveRole (IDType roleId) => RoleIDs.Remove (roleId);
}
