namespace congress_cucuta.Data;

internal class Simulation () {
    public History History { get; } = new History ();
    public List<Procedure> ProceduresGovernmental { get; } = []; 
    public List<Procedure> ProceduresSpecial { get; } = []; 
    public List<Procedure> ProceduresDeclared { get; } = [];
    public List<Region> Regions { get; } = [];
    public List<Party> Parties { get; } = [];
    public List<Ballot> Ballots { get; } = [];
}

internal class SimulationContext () {
    private readonly Dictionary<Role, int> roles = [];
    private readonly HashSet<Person> people = new (new IIDEqualityComparer ());
    private readonly HashSet<Region> regionsAll = new (new IIDEqualityComparer ());
    private readonly HashSet<Region> regionsActive = new (new IIDEqualityComparer ());
    private readonly HashSet<Party> partiesAll = new (new IIDEqualityComparer ());
    private readonly HashSet<Party> partiesActive = new (new IIDEqualityComparer ());
    private readonly HashSet<Procedure> proceduresAll = new (new IIDEqualityComparer ());
    private readonly HashSet<Procedure> proceduresActove = new (new IIDEqualityComparer ());
    private readonly HashSet<byte> ballotsPassed = [];
    private readonly Dictionary<byte, byte> currencyValues = [];
    private readonly HashSet<byte> activeProcedures = [];

    public void PassBallot (byte ballotID) => ballotsPassed.Add (ballotID);

    public void SetCurrencyValue (byte ownerID, byte value) => currencyValues[ownerID] = value;

    public void ActivateProcedure (byte procedureID) => activeProcedures.Add (procedureID);

    public void DeactivateProcedure (byte procedureID) => activeProcedures.Remove (procedureID);

    public bool IsBallotPassed (byte ballotID) => ballotsPassed.Contains (ballotID);

    public byte GetBallotsPassedCount () => (byte) ballotsPassed.Count;

    public byte GetCurrencyValue (byte ownerID) => currencyValues.GetValueOrDefault (ownerID);

    public bool IsProcedureActive (byte procedureID) => activeProcedures.Contains (procedureID);
}
