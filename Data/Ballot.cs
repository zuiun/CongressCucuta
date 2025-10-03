namespace congress_cucuta.Data;

internal readonly struct Ballot (
    IDType id,
    string title,
    string name,
    List<string> description,
    Ballot.Result passResult,
    Ballot.Result failResult,
    bool isIncident = false
) : IID {
    internal readonly struct Effect {
        internal enum ActionType {
            Add,
            Remove,
            Replace,
        }

        internal enum TargetType {
            Region,
            Party,
            Procedure,
        }

        public ActionType Action { get; }
        public TargetType Target { get; }
        public IDType TargetID { get; }
        public IDType? ReplacementID { get; }

        public Effect (ActionType action, TargetType target, IDType targetId, IDType? replacementId = null) {
            if (action is ActionType.Replace && replacementId is null)
                throw new ArgumentException ("replacementID must exist for action = Replace");

            Action = action;
            Target = target;
            TargetID = targetId;
            ReplacementID = replacementId;
        }
    }

    internal readonly struct Result (List<Effect> effects, List<string> description, List<Link<Ballot>> links, bool isPassed = true) {
        public bool IsPassed { get; } = isPassed;
        public List<Effect> Effects { get; } = effects;
        public List<string> Description { get; } = description;
        public List<Link<Ballot>> Links { get; } = links;
    }

    public IDType ID { get; } = id;
    public bool IsIncident { get; } = isIncident;
    // Short title of Ballot, eg Ballot A
    public string Title { get; } = title;
    // Actual name of Ballot
    public string Name { get; } = name;
    public List<string> Description { get; } = description;
    public Result PassResult { get; } = passResult;
    public Result FailResult { get; } = failResult;
}
