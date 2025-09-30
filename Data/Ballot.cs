namespace congress_cucuta.Data;

internal readonly struct BallotResult (List<IEffect> effects, List<string> description, bool isPassed = true) {
    public bool IsPassed { get; } = isPassed;
    public List<IEffect> Effects { get; } = effects;
    public List<string> Description { get; } = description;
}

internal readonly struct Ballot (
    byte id,
    string title,
    string name,
    List<string> description,
    BallotResult passResult,
    BallotResult failResult,
    List<Link<Ballot>> links,
    bool isIncident = false
) : IID {
    public byte ID { get; } = id;
    public bool IsIncident { get; } = isIncident;
    public string Title { get; } = title;
    public string Name { get; } = name;
    public List<string> Description { get; } = description;
    public BallotResult PassResult { get; } = passResult;
    public BallotResult FailResult { get; } = failResult;
    public List<Link<Ballot>> Links { get; } = links;
}
