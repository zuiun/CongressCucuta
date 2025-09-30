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
        Currency? currency = null
    ) {
        if (id == STATE) {
            throw new ArgumentOutOfRangeException (nameof (id), "id cannot be STATE (255)");
        }

        ID = id;
        Name = name;
        Description = description;
        Leader = leader;
        Currency = currency;
    }

    public byte ID { get; }
    string Name { get; }
    List<string> Description { get; }
    string? Leader { get; }
    Currency? Currency { get; }
}

internal class Party (
    byte id,
    string name,
    List<string> description,
    string leader,
    string? abbreviation = null,
    Currency? currency = null
) : Faction (id, name, description, leader, currency) {
    public string? Abbreviation { get; } = abbreviation;
}

internal class Region (
    byte id,
    string name,
    List<string> description,
    string? leader,
    Currency? currency = null
) : Faction (id, name, description, leader, currency) { }
