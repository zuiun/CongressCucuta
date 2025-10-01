namespace congress_cucuta.Data;

internal readonly struct HistoryPath (
    HashSet<IDType> ballotsPassed,
    Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared
) {
    public HashSet<IDType> BallotsPassed { get; } = ballotsPassed;
    public Dictionary<IDType, SortedSet<IDType>> BallotsProceduresDeclared { get; } = ballotsProceduresDeclared;
}

internal readonly struct History (
    string state,
    string government,
    string[] context,
    string date,
    string situation,
    string period,
    string member,
    string speaker,
    HistoryPath path
) {
    public string State { get; } = state;
    public string Government { get; } = government;
    public string[] Context { get; } = context;
    public string Date { get; } = date;
    public string Situation { get; } = situation;
    public string Period { get; } = period;
    public string Member { get; } = member;
    public string Speaker { get; } = speaker;
    public HistoryPath Path { get; } = path;
}
