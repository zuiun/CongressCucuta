namespace CongressCucuta.Data;

internal readonly record struct Person (IDType ID, string Name) : IID;
