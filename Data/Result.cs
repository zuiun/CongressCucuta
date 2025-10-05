namespace congress_cucuta.Data;

internal readonly record struct Result (IDType ID, List<Link<Result>> Links) : IID;
