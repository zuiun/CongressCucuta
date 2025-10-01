namespace congress_cucuta.Data;

internal class Simulation {
    public Simulation (
        History history,
        Dictionary<Role, Permissions> rolePermissions,
        List<Procedure> proceduresGovernmental,
        List<Procedure> proceduresSpecial,
        List<Procedure> proceduresDeclared,
        string regionNameSingular,
        string regionNamePlural,
        List<Region> regions,
        string partyNameSingular,
        string partyNamePlural,
        List<Party> parties,
        List<Ballot> ballots
    ) {
        History = history;
        RolePermissions = rolePermissions;
        ProceduresGovernmental = proceduresGovernmental;
        ProceduresSpecial = proceduresSpecial;
        ProceduresDeclared = proceduresDeclared;
        RegionNameSingular = regionNameSingular;
        RegionNamePlural = regionNamePlural;
        Regions = regions;
        PartyNameSingular = partyNameSingular;
        PartyNamePlural = partyNamePlural;
        Parties = parties;
        Ballots = ballots;

        Validate ();
    }

    public History History { get; }
    public Dictionary<Role, Permissions> RolePermissions { get; }
    public List<Procedure> ProceduresGovernmental { get; } 
    public List<Procedure> ProceduresSpecial { get; }
    public List<Procedure> ProceduresDeclared { get; }
    public string RegionNameSingular { get; }
    public string RegionNamePlural { get; }
    public List<Region> Regions { get; }
    public string PartyNameSingular { get; }
    public string PartyNamePlural { get; }
    public List<Party> Parties { get; }
    public List<Ballot> Ballots { get; }

    private void Validate () {
        throw new NotImplementedException ();
    }
}

internal class SimulationContext () {
    private static readonly IIDEqualityComparer equalityComparer = new ();
    private readonly Dictionary<Role, Permissions> rolesPermissions = [];
    private readonly HashSet<Person> people = new (equalityComparer);
    private readonly HashSet<Faction> factionsAll = new (equalityComparer);
    private readonly HashSet<IDType> factionsActive = [];
    private readonly HashSet<Procedure> proceduresAll = new (equalityComparer);
    private readonly HashSet<IDType> proceduresActive = [];
    private readonly HashSet<IDType> ballotsPassed = [];
    private readonly Dictionary<Currency, byte> currencyValues = new (equalityComparer);
    private readonly HashSet<IDType> activeProcedures = [];

    public IDType BallotCurrentID { get; set; } = 0;

    public void PassBallot (IDType ballotID) => ballotsPassed.Add (ballotID);

    public void SetCurrencyValue (IDType currencyID, byte value) => currencyValues[currencyID] = value;

    public void ActivateProcedure (IDType procedureID) => activeProcedures.Add (procedureID);

    public void DeactivateProcedure (IDType procedureID) => activeProcedures.Remove (procedureID);

    public bool IsBallotPassed (IDType ballotID) => ballotsPassed.Contains (ballotID);

    public byte GetBallotsPassedCount () => (byte) ballotsPassed.Count;

    public byte GetCurrencyValue (IDType currencyID) => currencyValues.GetValueOrDefault (currencyID);

    public bool IsProcedureActive (IDType procedureID) => activeProcedures.Contains (procedureID);
}
