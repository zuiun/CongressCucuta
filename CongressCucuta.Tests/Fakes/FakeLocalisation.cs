using CongressCucuta.Core;

namespace CongressCucuta.Tests.Unit.Fakes;

public static class FakeLocalisation {
    public static Localisation Create () {
        string state = "State";
        string government = "Government";
        string[] context = ["Context"];
        string date = "Date";
        string situation = "Situation";
        string period = "Period";
        Dictionary<IDType, (string, string)> roles = [];
        roles[Role.MEMBER] = ("Member", "Members");
        roles[Role.HEAD_GOVERNMENT] = ("Government Head", "Government Heads");
        roles[Role.HEAD_STATE] = ("State Head", "State Heads");
        roles[Role.LEADER_PARTY] = ("Party Leader", "Party Leaders");
        roles[Role.LEADER_REGION] = ("Region Leader", "Region Leaders");
        roles[0] = ("0 Leader", "0 Leaders");
        roles[1] = ("1 Leader", "1 Leaders");
        roles[2] = ("2 Leader", "2 Leaders");
        roles[3] = ("3 Leader", "3 Leaders");
        string speaker = "Speaker";
        (string, string) region = ("Region", "Regions");
        Dictionary<IDType, (string, string[])> regions = [];
        regions[0] = ("Region 0", []);
        regions[1] = ("Region 1", []);
        (string, string) party = ("Party", "Parties");
        Dictionary<IDType, (string, string[])> parties = [];
        parties[2] = ("Party 2", []);
        parties[3] = ("Party 3", []);
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[2] = "2";
        Dictionary<IDType, string> currencies = [];
        currencies[0] = "Currency 0";
        currencies[1] = "Currency 1";
        currencies[2] = "Currency 2";
        currencies[3] = "Currency 3";
        currencies[Currency.PARTY] = "Currency Party";
        currencies[Currency.REGION] = "Currency Region";
        currencies[Currency.STATE] = "Currency State";
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[0] = ("Procedure 0", Localisation.UNUSED);
        procedures[1] = ("Procedure 1", Localisation.UNUSED);
        procedures[2] = ("Procedure 2", Localisation.UNUSED);
        procedures[3] = ("Procedure 3", Localisation.UNUSED);
        Dictionary<IDType, (string, string, string[], string[], string[])> ballots = [];
        ballots[0] = ("0", "Ballot 0", [], [], []);
        ballots[1] = ("1", "Incident 1", [], [], []);
        Dictionary<IDType, (string, string[])> results = [];
        results[0] = ("Result", []);

        return new (
            state,
            government,
            context,
            date,
            situation,
            period,
            roles,
            speaker,
            region,
            regions,
            party,
            parties,
            abbreviations,
            currencies,
            procedures,
            ballots,
            results
        );
    }
}
