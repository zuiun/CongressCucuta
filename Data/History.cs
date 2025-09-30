namespace congress_cucuta.Data;

internal readonly struct HistoryPath (List<bool> ballotsPassed, List<List<byte>> proceduresDeclared) {
    public List<bool> BallotsPassed { get; } = ballotsPassed;
    public List<List<byte>> ProceduresDeclared { get; } = proceduresDeclared;
}

internal readonly struct History (
    string state,
    string government,
    string date,
    string situation,
    string period,
    string member,
    string speaker,
    HistoryPath path
) {
    public string State { get; } = state;
    public string Government { get; } = government;
    public string Date { get; } = date;
    public string Situation { get; } = situation;
    public string Period { get; } = period;
    public string Member { get; } = member;
    public string Speaker { get; } = speaker;
    public HistoryPath Path { get; } = path;
}
