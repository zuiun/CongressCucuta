namespace congress_cucuta.Data;

/*
 * Assumptions to make this easier to parse:
 * Everyone is assigned at least one role
 * Every role has permissions
 * Every permission has a vote number
 * Those who cannot vote will never be added to either Votes HashSet
 */
internal struct SimulationContext (SimulationContext.BallotContext ballot) {
    internal struct BallotContext (byte peopleCount, bool isSimpleMajority = true) {
        // 50% + 1
        private readonly byte _majoritySimple = (byte) (Math.Ceiling (peopleCount / 2m) + 1);
        // 2 / 3
        private readonly byte _majoritySuper = (byte) Math.Ceiling ((peopleCount * 2) / 3m);

        // This gets composed/replaced as necessary
        public Dictionary<IDType, Permissions> RolesPermissions { get; set; } = [];
        // This gets replaced as necessary
        public Dictionary<IDType, HashSet<IDType>> PeopleRoles { get; set; } = [];
        public HashSet<IDType> VotesPass { get; } = [];
        public HashSet<IDType> VotesFail { get; } = [];
        public bool IsSimpleMajority { get; } = isSimpleMajority;

        public readonly bool? IsBallotVoted () {
            byte majority = IsSimpleMajority ? _majoritySimple : _majoritySuper;
            byte passCount = 0;
            byte failCount = 0;

            foreach (IDType personId in VotesPass) {
                foreach (IDType roleId in PeopleRoles[personId]) {
                    passCount += RolesPermissions[roleId].Votes;
                }
            }

            foreach (IDType personId in VotesFail) {
                foreach (IDType roleId in PeopleRoles[personId]) {
                    failCount += RolesPermissions[roleId].Votes;
                }
            }

            if (passCount >= majority) {
                return true;
            } else if (failCount >= majority) {
                return false;
            } else {
                return null;
            }
        }

        public readonly void UpdateRolePermissions (IDType roleID, Permissions.Composition permissions) => RolesPermissions[roleID] += permissions;
    }

    private readonly Dictionary<IDType, Permissions> _rolesPermissions = [];
    private readonly Dictionary<IDType, HashSet<IDType>> _peopleRoles = [];
    private readonly Dictionary<IDType, HashSet<IDType>> _peopleFactions = [];
    private readonly Dictionary<IDType, sbyte> _currencyValues = [];
    private readonly HashSet<IDType> _partiesActive = [];
    private readonly HashSet<IDType> _regionsActive = [];
    private readonly HashSet<IDType> _proceduresActive = [];
    private readonly HashSet<IDType> _ballotsPassed = [];
    private readonly BallotContext _ballot = ballot;
    public IDType BallotCurrentID { get; set; } = 0;
    public Dictionary<IDType, Role> Roles { get; } = [];
    public Dictionary<IDType, Person> People { get; } = [];
    public Dictionary<IDType, Currency> Currencies { get; } = [];
    public Dictionary<IDType, Party> Parties { get; } = [];
    public Dictionary<IDType, Region> Regions { get; } = [];
    public Dictionary<IDType, Procedure> Procedures { get; } = [];
    public Dictionary<IDType, Ballot> Ballots { get; } = [];

    public readonly void PassBallot (IDType ballotId) => _ballotsPassed.Add (ballotId);

    public readonly void SetCurrencyValue (IDType currencyId, sbyte value) => _currencyValues[currencyId] = value;

    public readonly void ActivateProcedure (IDType procedureId) => _proceduresActive.Add (procedureId);

    public readonly void DeactivateProcedure (IDType procedureId) => _proceduresActive.Remove (procedureId);

    public readonly bool? IsBallotVoted () => _ballot.IsBallotVoted ();

    public readonly bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public readonly byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public readonly sbyte GetCurrencyValue (IDType currencyId) => _currencyValues.GetValueOrDefault (currencyId);

    public readonly bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
