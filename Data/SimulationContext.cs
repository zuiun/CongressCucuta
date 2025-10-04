namespace congress_cucuta.Data;

/*
 * Assumptions to make this easier to parse:
 * Everyone is assigned at least one role
 * Those who cannot vote will never be added to either Votes HashSet
 */
internal class SimulationContext {
    internal class BallotContext (byte peopleCount, bool isSimpleMajority = true) {
        // 50% + 1
        private readonly byte _majoritySimple = (byte) Math.Ceiling (peopleCount / 2m);
        // 2 / 3
        private readonly byte _majoritySuper = (byte) Math.Ceiling ((peopleCount * 2) / 3m);

        // This gets composed/replaced as necessary
        public Dictionary<IDType, Permissions> RolesPermissions { get; set; } = [];
        // This gets replaced as necessary
        public Dictionary<IDType, HashSet<IDType>> PeopleRoles { get; set; } = [];
        public HashSet<IDType> VotesPass => [];
        public HashSet<IDType> VotesFail => [];
        public bool IsSimpleMajority { get; set; } = isSimpleMajority;

        public bool? IsBallotVoted () {
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

        public void UpdateRolePermissions (IDType roleID, Permissions.Composition permissions) => RolesPermissions[roleID] += permissions;
    }

    private readonly Dictionary<IDType, Permissions> _rolesPermissions = [];
    private readonly Dictionary<IDType, HashSet<IDType>> _peopleRoles = [];
    private readonly Dictionary<IDType, HashSet<IDType>> _peopleFactions = [];
    private readonly Dictionary<IDType, sbyte> _currenciesValues = [];
    private readonly HashSet<IDType> _partiesActive = [];
    private readonly HashSet<IDType> _regionsActive = [];
    private readonly HashSet<IDType> _proceduresActive = [];
    private readonly HashSet<IDType> _ballotsPassed = [];
    public IDType BallotCurrentID { get; set; } = 0;
    public Dictionary<IDType, Role> Roles => [];
    public Dictionary<IDType, Person> People => [];
    public Dictionary<IDType, Faction> Parties => [];
    public Dictionary<IDType, Faction> Regions => [];
    public Dictionary<IDType, Currency> Currencies => [];
    public Dictionary<IDType, Procedure> Procedures => [];
    public Dictionary<IDType, Ballot> Ballots => [];
    public BallotContext? Ballot { get; set; }

    public void PassBallot (IDType ballotId) => _ballotsPassed.Add (ballotId);

    public void SetCurrencyValue (IDType currencyId, sbyte value) => _currenciesValues[currencyId] = value;

    public void ActivateProcedure (IDType procedureId) => _proceduresActive.Add (procedureId);

    public void DeactivateProcedure (IDType procedureId) => _proceduresActive.Remove (procedureId);

    public bool? IsBallotVoted () => Ballot?.IsBallotVoted ();

    public bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public sbyte GetCurrencyValue (IDType currencyId) => _currenciesValues.GetValueOrDefault (currencyId);

    public bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
