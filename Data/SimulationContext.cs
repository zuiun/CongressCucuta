namespace congress_cucuta.Data;

// Must call InitialisePeople () after construction to complete initialisation
internal class SimulationContext (Simulation simulation) {
    internal class BallotContext (bool isSimpleMajority = true) {
        // Replaced as necessary (every ballot, upon VotersLimit declared, upon role change)
        public Dictionary<IDType, Permissions> PeoplePermissions { get; set; } = [];
        public HashSet<IDType> VotesPass { get; } = [];
        public HashSet<IDType> VotesFail { get; } = [];
        public bool IsSimpleMajority { get; set; } = isSimpleMajority;

        public byte CalculateVotesTotal () {
            byte votes = 0;

            foreach (Permissions p in PeoplePermissions.Values) {
                if (p.CanVote) {
                    votes += p.Votes;
                }
            }

            return votes;
        }

        public byte CalculateVotesMajority () {
            byte votesTotal = CalculateVotesTotal ();

            return IsSimpleMajority
                ? (byte) (Math.Floor (votesTotal / 2m) + 1)
                : (byte) Math.Ceiling ((votesTotal * 2) / 3m);
        } 

        public bool? IsBallotVoted () {
            byte votesPass = CalculateVotesMajority ();
            byte passCount = 0;
            byte failCount = 0;

            foreach (IDType p in VotesPass) {
                passCount += PeoplePermissions[p].Votes;
            }

            foreach (IDType p in VotesFail) {
                failCount += PeoplePermissions[p].Votes;
            }

            if (passCount >= votesPass) {
                return true;
            } else if (failCount >= votesPass) {
                return false;
            } else {
                return null;
            }
        }

        public void UpdatePersonPermissions (IDType personId, Permissions permissions) => PeoplePermissions[personId] = permissions;
    }

    private readonly Dictionary<IDType, SortedSet<IDType>> _peopleRoles = [];
    private readonly Dictionary<IDType, HashSet<IDType>> _peopleFactions = [];
    private readonly Dictionary<IDType, Permissions> _rolesPermissions = simulation.RolesPermissions.ToDictionary (k => k.Key, k => k.Value);
    private readonly HashSet<IDType> _partiesActive = [.. simulation.Parties.Where (p => p.IsActiveStart).Select (p => p.ID)];
    private readonly HashSet<IDType> _regionsActive = [.. simulation.Regions.Where (r => r.IsActiveStart).Select (r => r.ID)];
    private readonly HashSet<IDType> _proceduresActive = [
        .. simulation.ProceduresGovernmental.Select (pi => pi.ID),
        .. simulation.ProceduresSpecial.Select (pt => pt.ID),
        .. simulation.ProceduresDeclared.Select (pd => pd.ID),
    ];
    private readonly HashSet<IDType> _ballotsPassed = [];
    public IDType BallotCurrentID { get; set; } = 0;
    public Dictionary<IDType, Person> People { get; } = [];
    public HashSet<IDType> Roles { get; } = [.. simulation.RolesPermissions.Keys];
    public Dictionary<IDType, Faction> Parties { get; } = simulation.Parties.ToDictionary (p => p.ID, p => p);
    public Dictionary<IDType, Faction> Regions { get; } = simulation.Regions.ToDictionary (r => r.ID, r => r);
    public HashSet<IDType> Currencies { get; } = [.. simulation.CurrenciesValues.Keys];
    public Dictionary<IDType, sbyte> CurrenciesValues { get; } = simulation.CurrenciesValues;
    public Dictionary<IDType, ProcedureImmediate> ProceduresGovernmental { get; } = simulation.ProceduresGovernmental.ToDictionary (pi => pi.ID, pi => pi);
    public Dictionary<IDType, ProcedureTargeted> ProceduresSpecial { get; } = simulation.ProceduresSpecial.ToDictionary (pt => pt.ID, pt => pt);
    public Dictionary<IDType, ProcedureDeclared> ProceduresDeclared { get; } = simulation.ProceduresDeclared.ToDictionary (pd => pd.ID, pd => pd);
    public Dictionary<IDType, Ballot> Ballots { get; } = simulation.Ballots.ToDictionary (b => b.ID, b => b);
    public BallotContext? Ballot { get; set; }

    public void InitialisePeople (List<Person> people) {
        foreach (Person p in people) {
            People[p.ID] = p;
            _peopleRoles[p.ID] = [Role.MEMBER];
            _peopleFactions[p.ID] = [];
        }
    }

    public void AddPersonRole (IDType personId, IDType roleId) => _peopleRoles[personId].Add (roleId);

    public void RemovePersonRole (IDType personId, IDType roleId) => _peopleRoles[personId].Remove (roleId);

    public void AddPersonFaction (IDType personId, IDType factionId) => _peopleRoles[personId].Add (factionId);

    public void RemovePersonFaction (IDType personId, IDType factionId) => _peopleRoles[personId].Add (factionId);

    public void PassBallot (IDType ballotId) => _ballotsPassed.Add (ballotId);

    public void SetCurrencyValue (IDType currencyId, sbyte value) => CurrenciesValues[currencyId] = value;

    public void ActivateProcedure (IDType procedureId) => _proceduresActive.Add (procedureId);

    public void DeactivateProcedure (IDType procedureId) => _proceduresActive.Remove (procedureId);

    public bool? IsBallotVoted () => Ballot?.IsBallotVoted ();

    public bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public sbyte GetCurrencyValue (IDType currencyId) => CurrenciesValues.GetValueOrDefault (currencyId);

    public bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
