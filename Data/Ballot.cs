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
            Currency
        }

        public ActionType Action { get; }
        public TargetType Target { get; }
        public IDType TargetID { get; }
        public IDType? ReplacementID { get; }
        public byte? Value { get; }

        public Effect (ActionType action, TargetType target, IDType targetId, IDType? replacementId = null, byte? value = null) {
            if (action is ActionType.Replace && replacementId is null) {
                throw new ArgumentException ("replacementID must exist for ActionType Replace", nameof (replacementId));
            }

            if (target is TargetType.Currency && value is null) {
                throw new ArgumentException ("value must exist for TargetType Currency", nameof (value));
            }

            Action = action;
            Target = target;
            TargetID = targetId;
            ReplacementID = replacementId;
            Value = value;
        }
    }

    internal readonly struct Result (List < Effect> effects, List<string> description, List<Link<Ballot>> links) {
        public List<Effect> Effects => effects;
        public List<string> Description => description;
        public List<Link<Ballot>> Links => links;
    }

    public IDType ID => id;
    public bool IsIncident => isIncident;
    // Short title of Ballot, eg Ballot A
    public string Title => title;
    // Actual name of Ballot
    public string Name => name;
    public List<string> Description => description;
    public Result PassResult => passResult;
    public Result FailResult => failResult;
}
