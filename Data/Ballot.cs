namespace congress_cucuta.Data;

internal readonly struct Ballot (
    byte id,
    string title,
    string name,
    List<string> description,
    Ballot.Result passResult,
    Ballot.Result failResult,
    List<Link<Ballot>> links,
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

        public Effect (ActionType action, TargetType target, byte targetID, byte? replacementID = null) {
            if (action is ActionType.Replace && replacementID is null) {
                throw new ArgumentException ("replacementID must exist for action = Replace");
            }

            Action = action;
            Target = target;
            TargetID = targetID;
            ReplacementID = replacementID;
        }

        public ActionType Action { get; }
        public TargetType Target { get; }
        public byte TargetID { get; }
        public byte? ReplacementID { get; }
    }

    internal readonly struct Result (List<Effect> effects, List<string> description, bool isPassed = true) {
        public bool IsPassed { get; } = isPassed;
        public List<Effect> Effects { get; } = effects;
        public List<string> Description { get; } = description;
    }

    public byte ID { get; } = id;
    public bool IsIncident { get; } = isIncident;
    public string Title { get; } = title;
    public string Name { get; } = name;
    public List<string> Description { get; } = description;
    public Ballot.Result PassResult { get; } = passResult;
    public Ballot.Result FailResult { get; } = failResult;
    public List<Link<Ballot>> Links { get; } = links;
}
