namespace CongressCucuta.Core;

public readonly record struct Result (IDType ID, List<Link<Result>> Links) : IID;
