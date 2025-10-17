namespace CongressCucuta.Data;

internal readonly record struct Localisation (
    string State,
    string Government,
    string[] Context,
    string Date,
    string Situation,
    string Period,
    // (singular, plural)
    Dictionary<IDType, (string, string)> Roles,
    string Speaker,
    // (singular, plural)
    (string, string) Region,
    // (name, description)
    Dictionary<IDType, (string, string[])> Regions,
    (string, string) Party,
    // (name, description)
    Dictionary<IDType, (string, string[])> Parties,
    Dictionary<IDType, string> Abbreviations,
    Dictionary<IDType, string> Currencies,
    // (name, description)
    Dictionary<IDType, (string, string)> Procedures,
    // (title, name, description, pass, fail)
    Dictionary<IDType, (string, string, string[], string[], string[])> Ballots,
    // (title, description)
    Dictionary<IDType, (string, string[])> Results
) {
    public const string UNUSED = "";

    public string GetFactionAndAbbreviation (IDType factionId) {
        string name;

        if (Parties.TryGetValue (factionId, out (string, string[]) party)) {
            name = party.Item1;
        } else if (Regions.TryGetValue (factionId, out (string, string[]) region)) {
            name = region.Item1;
        } else {
            throw new ArgumentException ($"No Faction ID matches ID {factionId}", nameof (factionId));
        }

        return Abbreviations.TryGetValue (factionId, out string? abbreviation) ? $"{name} ({abbreviation})" : name;
    }

    public string GetFactionOrAbbreviation (IDType factionId) {
        if (Abbreviations.TryGetValue (factionId, out string? abbreviation)) {
            return abbreviation;
        } else if (Parties.TryGetValue (factionId, out (string, string[]) party)) {
            return party.Item1;
        } else if (Regions.TryGetValue (factionId, out (string, string[]) region)) {
            return region.Item1;
        } else {
            throw new ArgumentException ($"No Faction ID matches ID {factionId}", nameof (factionId));
        }
    }
}
