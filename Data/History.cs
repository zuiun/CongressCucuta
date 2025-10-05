namespace congress_cucuta.Data;

internal readonly record struct History (
    HashSet<IDType> BallotsPassed,
    Dictionary<IDType, SortedSet<IDType>> BallotsProceduresDeclared
);
