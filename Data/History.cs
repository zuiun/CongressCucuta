namespace congress_cucuta.Data;

internal readonly struct History (
    HashSet<IDType> ballotsPassed,
    Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared
) {
    public HashSet<IDType> BallotsPassed => ballotsPassed;
    public Dictionary<IDType, SortedSet<IDType>> BallotsProceduresDeclared => ballotsProceduresDeclared;
}
