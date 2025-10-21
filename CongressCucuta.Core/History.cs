namespace CongressCucuta.Core;

public readonly record struct History (
    HashSet<IDType> BallotsPassed,
    Dictionary<IDType, SortedSet<IDType>> BallotsProceduresDeclared
);
