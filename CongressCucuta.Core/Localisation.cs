using System.Text.Json.Serialization;

namespace CongressCucuta.Core;

public readonly record struct Localisation {
    public const string UNUSED = "";
    public readonly string State;
    public readonly string Government;
    public readonly string[] Context;
    public readonly string Date;
    public readonly string Situation;
    public readonly string Period;
    // (singular, plural)
    public readonly Dictionary<IDType, (string, string)> Roles;
    public readonly string Speaker;
    // (singular, plural)
    public readonly (string, string) Region;
    // (name, description)
    public readonly Dictionary<IDType, (string, string[])> Regions;
    public readonly (string, string) Party;
    // (name, description)
    public readonly Dictionary<IDType, (string, string[])> Parties;
    public readonly Dictionary<IDType, string> Abbreviations;
    public readonly Dictionary<IDType, string> Currencies;
    // (name, description)
    public readonly Dictionary<IDType, (string, string)> Procedures;
    // (title, name, description, pass, fail)
    public readonly Dictionary<IDType, (string, string, string[], string[], string[])> Ballots;
    // (title, description)
    public readonly Dictionary<IDType, (string, string[])> Results;

    [JsonConstructor]
    public Localisation (
        string state,
        string government,
        string[] context,
        string date,
        string situation,
        string period,
        Dictionary<IDType, (string, string)> roles,
        string speaker,
        (string, string) region,
        Dictionary<IDType, (string, string[])> regions,
        (string, string) party,
        Dictionary<IDType, (string, string[])> parties,
        Dictionary<IDType, string> abbreviations,
        Dictionary<IDType, string> currencies,
        Dictionary<IDType, (string, string)> procedures,
        Dictionary<IDType, (string, string, string[], string[], string[])> ballots,
        Dictionary<IDType, (string, string[])> results
    ) {
        if (string.IsNullOrWhiteSpace (state)) {
            throw new ArgumentException ("State must be populated", nameof (state));
        }

        if (string.IsNullOrWhiteSpace (government)) {
            throw new ArgumentException ("Government must be populated", nameof (government));
        }

        if (context.Length == 0 || context.Any (l => string.IsNullOrWhiteSpace (l))) {
            throw new ArgumentException ("Context must be populated", nameof (context));
        }

        if (string.IsNullOrWhiteSpace (date)) {
            throw new ArgumentException ("Date must be populated", nameof (date));
        }

        if (string.IsNullOrWhiteSpace (situation)) {
            throw new ArgumentException ("Situation must be populated", nameof (situation));
        }

        if (string.IsNullOrWhiteSpace (period)) {
            throw new ArgumentException ("Period must be populated", nameof (period));
        }

        if (string.IsNullOrWhiteSpace (speaker)) {
            throw new ArgumentException ("Speaker must be populated", nameof (speaker));
        }

        State = state;
        Government = government;
        Context = context;
        Date = date;
        Situation = situation;
        Period = period;
        Roles = roles;
        Speaker = speaker;
        Region = region;
        Regions = regions;
        Party = party;
        Parties = parties;
        Abbreviations = abbreviations;
        Currencies = currencies;
        Procedures = procedures;
        Ballots = ballots;
        Results = results;
    }

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
