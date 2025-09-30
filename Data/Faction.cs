namespace congress_cucuta.Data;

/*
 * Factions cannot share IDs due to Role assignment logic
 * eg Party.ID = 0 and Region.ID = 0 is an error
 * This is not enforced in code
 * TODO: Enforce this at Simulation creation
 */
internal abstract class Faction : IID {
    public const byte STATE = byte.MaxValue;

    public Faction (
        byte id,
        string name,
        List<string> description,
        string? leader,
        bool isActiveStart = true
    ) {
        if (id == STATE) {
            throw new ArgumentOutOfRangeException (nameof (id), "id cannot be STATE (255)");
        }

        ID = id;
        IsActiveStart = isActiveStart;
        Name = name;
        Description = description;
        Leader = leader;
    }

    public byte ID { get; }
    public bool IsActiveStart { get; }
    public string Name { get; }
    public List<string> Description { get; }
    public string? Leader { get; }
}

internal class Party (
    byte id,
    string name,
    List<string> description,
    string leader,
    bool isActiveStart = true,
    string? abbreviation = null
) : Faction (id, name, description, leader, isActiveStart) {
    public string? Abbreviation { get; } = abbreviation;
}

internal class Region (
    byte id,
    string name,
    List<string> description,
    string? leader,
    bool isActiveStart = true
) : Faction (id, name, description, leader, isActiveStart) { }
