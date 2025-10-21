namespace CongressCucuta.Core;

public readonly record struct Person (IDType ID, string Name) : IID;
