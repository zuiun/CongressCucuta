namespace congress_cucuta.Data;

internal readonly struct Localisation (
    string state,
    string government,
    string[] context,
    string date,
    string situation,
    string period,
    Dictionary<IDType, (string, string)> roles,
    (string, string) member,
    string speaker,
    (string, string) region,
    Dictionary<IDType, (string, string[], string)> regions,
    (string, string) party,
    Dictionary<IDType, (string, string[], string)> parties,
    Dictionary<IDType, string> abbreviations,
    Dictionary<IDType, string> currencies,
    Dictionary<IDType, (string, string)> procedures,
    Dictionary<IDType, (string, string, string[], string[], string[])> ballots,
    Dictionary<IDType, (string, string[])> results
) {
    public const string UNUSED = "";

    public string State => state;
    public string Government => government;
    public string[] Context => context;
    public string Date => date;
    public string Situation => situation;
    public string Period => period;
    // (singular, plural)
    public Dictionary<IDType, (string, string)> Roles => roles;
    public (string, string) Member => member;
    public string Speaker => speaker;
    public (string, string) Region => region;
    // (name, description, leader)
    public Dictionary<IDType, (string, string[], string)> Regions => regions;
    // (singular, plural)
    public (string, string) Party => party;
    // (name, description, leader)
    public Dictionary<IDType, (string, string[], string)> Parties => parties;
    public Dictionary<IDType, string> Abbreviations => abbreviations;
    public Dictionary<IDType, string> Currencies => currencies;
    // (name, description)
    public Dictionary<IDType, (string, string)> Procedures => procedures;
    // (title, name, description, pass, fail)
    public Dictionary<IDType, (string, string, string[], string[], string[])> Ballots => ballots;
    // (title, description)
    public Dictionary<IDType, (string, string[])> Results => results;

    public string GetFactionAndAbbreviation (IDType factionId) {
        string name;

        if (Parties.TryGetValue (factionId, out (string, string[], string) party)) {
            name = party.Item1;
        } else if (Regions.TryGetValue (factionId, out (string, string[], string) region)) {
            name = region.Item1;
        } else {
            throw new ArgumentException ($"No Faction ID matches ID {factionId}", nameof (factionId));
        }

        return Abbreviations.TryGetValue (factionId, out string? abbreviation) ? $"{name} ({abbreviation})" : name;
    }

    public string GetFactionOrAbbreviation (IDType factionId) {
        if (Abbreviations.TryGetValue (factionId, out string? abbreviation)) {
            return abbreviation;
        } else if (Parties.TryGetValue (factionId, out (string, string[], string) party)) {
            return party.Item1;
        } else if (Regions.TryGetValue (factionId, out (string, string[], string) region)) {
            return region.Item1;
        } else {
            throw new ArgumentException ($"No Faction ID matches ID {factionId}", nameof (factionId));
        }
    }
}
