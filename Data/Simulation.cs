namespace congress_cucuta.Data;

internal class Simulation () {
    public History History { get; } = new History ();
    public List<ProcedureGovernmental> ProceduresGovernmental { get; } = []; 
    public List<ProcedureSpecial> ProceduresSpecial { get; } = []; 
    public List<ProcedureDeclared> ProceduresDeclared { get; } = [];
    public List<Region> Regions { get; } = [];
    public List<Party> Parties { get; } = [];
    public List<Ballot> Ballots { get; } = [];
}

internal class SimulationContext () {
    private readonly HashSet<byte> ballotsPassed = [];
    private readonly Dictionary<byte, byte> currencyValues = [];
    private readonly HashSet<byte> activeProcedures = [];

    public void PassBallot (byte ballotID) {
        ballotsPassed.Add (ballotID);
    }

    public void SetCurrencyValue (byte ownerID, byte value) {
        currencyValues[ownerID] = value;
    }

    public void ActivateProcedure (byte procedureID) {
        activeProcedures.Add (procedureID);
    }

    public void DeactivateProcedure (byte procedureID) {
        activeProcedures.Remove (procedureID);
    }

    public bool IsBallotPassed (byte ballotID) {
        return ballotsPassed.Contains (ballotID);
    }

    public byte GetBallotsPassedCount () {
        return (byte) ballotsPassed.Count;
    }

    public byte GetCurrencyValue (byte ownerID) {
        return currencyValues.GetValueOrDefault (ownerID);
    }

    public bool IsProcedureActive (byte procedureID) {
        return activeProcedures.Contains (procedureID);
    }
}
