namespace congress_cucuta.Data;

internal class Simulation () {
    public History History { get; } = new History ();
    public List<Procedure> ProceduresGovernmental { get; } = []; 
    public List<Procedure> ProceduresSpecial { get; } = []; 
    public List<Procedure> ProceduresDeclared { get; } = [];
    public string RegionNameSingular = "Region";
    public string RegionNamePlural = "Regions";
    public List<Region> Regions { get; } = [];
    public string PartyNameSingular = "Party";
    public string PartyNamePlural = "Parties";
    public List<Party> Parties { get; } = [];
    public List<Ballot> Ballots { get; } = [];
}

internal class SimulationContext () {
    private static readonly IIDEqualityComparer equalityComparer = new ();
    private readonly Dictionary<Role, Permissions> rolesPermissions = [];
    private readonly HashSet<Person> people = new (equalityComparer);
    private readonly HashSet<Faction> factionsAll = new (equalityComparer);
    private readonly HashSet<byte> factionsActive = [];
    private readonly HashSet<Procedure> proceduresAll = new (equalityComparer);
    private readonly HashSet<byte> proceduresActive = [];
    private readonly HashSet<byte> ballotsPassed = [];
    private readonly Dictionary<Currency, byte> currencyValues = new (equalityComparer);
    private readonly HashSet<byte> activeProcedures = [];

    public byte BallotCurrent { get; set; } = 0;

    public void PassBallot (byte ballotID) => ballotsPassed.Add (ballotID);

    public void SetCurrencyValue (byte currencyID, byte value) => currencyValues[currencyID] = value;

    public void ActivateProcedure (byte procedureID) => activeProcedures.Add (procedureID);

    public void DeactivateProcedure (byte procedureID) => activeProcedures.Remove (procedureID);

    public bool IsBallotPassed (byte ballotID) => ballotsPassed.Contains (ballotID);

    public byte GetBallotsPassedCount () => (byte) ballotsPassed.Count;

    public byte GetCurrencyValue (byte currencyID) => currencyValues.GetValueOrDefault (currencyID);

    public bool IsProcedureActive (byte procedureID) => activeProcedures.Contains (procedureID);
}
