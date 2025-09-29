namespace congress_cucuta.Data;

internal class Simulation () {
    public string State { get; } = "State";
    public string Government { get; } = "Government";
    public string Date { get; } = "Date";
    public string Event { get; } = "Event";
    public string Period { get; } = "Period";
    public List<ProcedureGovernmental> ProceduresGovernmental { get; } = []; 
    public List<ProcedureSpecial> ProceduresSpecial { get; } = []; 
    public List<ProcedureDeclared> ProceduresDeclared { get; } = [];
    public string TitleMembers { get; } = "Members";
    public string TitleSpeaker { get; } = "Speaker";
    // TODO: regions and parties
    // TODO: ballots list
    // TODO: ballots
    // TODO: end results
    // TODO: historical path
}
