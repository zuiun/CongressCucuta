namespace congress_cucuta.Data;

/*
 * Factions cannot share IDs due to Role assignment logic
 * eg Party.ID = 0 and Region.ID = 0 is an error
 * This is not enforced in code
 * TODO: Enforce this at Simulation creation
 */
internal abstract class Faction : IID {
    public IDType ID { get; }
    public bool IsActiveStart { get; }
    public string Name { get; }
    public List<string> Description { get; }
    public string? Leader { get; }

    protected Faction (
        IDType id,
        string name,
        List<string> description,
        string? leader,
        bool isActiveStart = true
    ) {
        if (id == Role.MEMBER || id == Role.HEAD_GOVERNMENT || id == Role.HEAD_STATE) {
            throw new ArgumentException ($"Faction ID {id} is reserved by Role", nameof (id));
        }

        ID = id;
        IsActiveStart = isActiveStart;
        Name = name;
        Description = description;
        Leader = leader;
    }
}

internal class Party (
    IDType id,
    string name,
    List<string> description,
    string leader,
    bool isActiveStart = true,
    string? abbreviation = null
) : Faction (id, name, description, leader, isActiveStart) {
    public string? Abbreviation => abbreviation;
}

internal class Region (
    IDType id,
    string name,
    List<string> description,
    string? leader = null,
    bool isActiveStart = true
) : Faction (id, name, description, leader, isActiveStart) { }
