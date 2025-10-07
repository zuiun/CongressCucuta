namespace congress_cucuta.Data;

internal class PreparingElectionEventArgs {
    public List<Election> Elections;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRoles;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactions;
    public HashSet<IDType> PartiesActive;
    public HashSet<IDType> RegionsActive;
    public Dictionary<IDType, Person> People;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRolesNew = [];
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactionsNew = [];

    public PreparingElectionEventArgs (
        List<Election> elections,
        ref readonly Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        ref readonly Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        ref readonly HashSet<IDType> partiesActive,
        ref readonly HashSet<IDType> regionsActive,
        ref readonly Dictionary<IDType, Person> people
    ) {
        elections.Sort ();
        Elections = elections;
        PeopleRoles = peopleRoles;
        PeopleFactions = peopleFactions;
        PartiesActive = partiesActive;
        RegionsActive = regionsActive;
        People = people;
    }
}

// Must call InitialisePeople () after construction to complete initialisation
internal class SimulationContext (Simulation simulation) {
    // TODO: not sure that this is necessary. it could definitely be done in the parent, though it would be very annoying
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

    private readonly Dictionary<IDType, Person> _people = [];
    private Dictionary<IDType, SortedSet<IDType>> _peopleRoles = [];
    // (party, region)
    private Dictionary<IDType, (IDType?, IDType?)> _peopleFactions = [];
    private readonly Dictionary<IDType, Permissions> _rolesPermissions = simulation.RolesPermissions.ToDictionary (k => k.Key, k => k.Value);
    private readonly HashSet<IDType> _partiesActive = [.. simulation.Parties.Where (p => p.IsActiveStart).Select (p => p.ID)];
    private readonly HashSet<IDType> _regionsActive = [.. simulation.Regions.Where (r => r.IsActiveStart).Select (r => r.ID)];
    private readonly HashSet<IDType> _proceduresActive = [
        //.. simulation.ProceduresGovernmental.Where (pi => pi.IsActiveStart).Select (pi => pi.ID),
        .. simulation.ProceduresSpecial.Where (pt => pt.IsActiveStart).Select (pt => pt.ID),
        //.. simulation.ProceduresDeclared.Where (pd => pd.IsActiveStart).Select (pd => pd.ID),
    ];
    private readonly HashSet<IDType> _ballotsPassed = [];
    private IDType _ballotCurrentId = 0;
    public IDType BallotCurrentID {
        get => _ballotCurrentId;
        set {
            _ballotCurrentId = value;
            StartBallot ();
        }
    }
    public Dictionary<IDType, Person> People => _people;
    public HashSet<IDType> Roles { get; } = [.. simulation.RolesPermissions.Keys];
    public Dictionary<IDType, Faction> Parties { get; } = simulation.Parties.ToDictionary (p => p.ID, p => p);
    public Dictionary<IDType, Faction> Regions { get; } = simulation.Regions.ToDictionary (r => r.ID, r => r);
    public HashSet<IDType> Currencies { get; } = [.. simulation.CurrenciesValues.Keys];
    public Dictionary<IDType, sbyte> CurrenciesValues { get; } = simulation.CurrenciesValues;
    public Dictionary<IDType, ProcedureImmediate> ProceduresGovernmental { get; } = simulation.ProceduresGovernmental.ToDictionary (pi => pi.ID, pi => pi);
    public Dictionary<IDType, ProcedureTargeted> ProceduresSpecial { get; } = simulation.ProceduresSpecial.ToDictionary (pt => pt.ID, pt => pt);
    public Dictionary<IDType, ProcedureDeclared> ProceduresDeclared { get; } = simulation.ProceduresDeclared.ToDictionary (pd => pd.ID, pd => pd);
    public Dictionary<IDType, Ballot> Ballots { get; } = simulation.Ballots.ToDictionary (b => b.ID, b => b);
    public BallotContext? Context { get; set; }
    public event EventHandler<PreparingElectionEventArgs>? PreparingElection;

    private void ComposePermissions () {
        // TODO
    }

    private void OnPrepareElection (List<Election> elections) {
        PreparingElectionEventArgs args = new (elections, in _peopleRoles, in _peopleFactions, in _partiesActive, in _regionsActive, in _people);

        PreparingElection?.Invoke (this, args);
        _peopleRoles = args.PeopleRolesNew;
        _peopleFactions = args.PeopleFactionsNew;
    }

    public void StartSetup () {
        List<(IDType, Procedure.EffectBundle)> effects = [
            .. ProceduresGovernmental.Values.Where (pi => pi.IsActiveStart)
                .Select (pi => (pi.ID, (Procedure.EffectBundle) pi.YieldEffects (0)!))
        ];
        List<Election> elections = effects.ConvertAll (e => new Election (e.Item1, e.Item2));

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }

        ComposePermissions ();
    }

    private void StartBallot () {
        // These are guaranteed to be ProcedureTargeted
        var effects = _proceduresActive.Select (pt => ProceduresSpecial[pt])
            .Select (pt => pt.YieldEffects (BallotCurrentID))
            .Where (e => e is not null)
            .Select (e => (Procedure.EffectBundle) e!);
        List<(IDType, Procedure.EffectBundle)> effectsElections = [];

        foreach (Procedure.EffectBundle e in effects) {

        // TODO: Apply the effects
        }

        if (effectsElections.Count > 0) {
            List<Election> elections = effectsElections.ConvertAll (e => new Election (e.Item1, e.Item2));

            // TODO: Invoke election... how?
            // could be two-level events.
            OnPrepareElection (elections);
        }

        // TODO: create context
        Context = new ();
    }

    private void EndBallot (bool isPass) {
        List<Ballot.Effect> effects;
        List<Election> elections = [];

        if (isPass) {
            effects = Ballots[BallotCurrentID].PassResult.Effects;
            _ballotsPassed.Add (BallotCurrentID);
        } else {
            effects = Ballots[BallotCurrentID].FailResult.Effects;
        }

        foreach (Ballot.Effect e in effects) {
            switch (e.Action) {
                case Ballot.Effect.ActionType.FoundParty:
                    foreach (IDType p in e.TargetIDs) {
                        _partiesActive.Add (p);
                    }

                    elections.Add (new (Election.ElectionType.ShuffleAdd, e.TargetIDs));
                    break;
                case Ballot.Effect.ActionType.DissolveParty:
                    foreach (IDType p in e.TargetIDs) {
                        _partiesActive.Remove (p);
                    }

                    elections.Add (new (Election.ElectionType.ShuffleRemove, e.TargetIDs));
                    break;
                case Ballot.Effect.ActionType.RemoveProcedure:
                    _proceduresActive.RemoveWhere (p => e.TargetIDs.Contains (p));
                    break;
                case Ballot.Effect.ActionType.ReplaceProcedure:
                    _proceduresActive.Remove (e.TargetIDs[0]);
                    _proceduresActive.Add (e.TargetIDs[1]);
                    break;
                case Ballot.Effect.ActionType.ModifyCurrency:
                    if (e.TargetIDs[0] == Currency.STATE) {
                        CurrenciesValues[Currency.STATE] += e.Value;
                    } else if (e.TargetIDs[0] == Currency.PARTY) {
                        foreach (IDType c in CurrenciesValues.Keys.Where (c => _partiesActive.Contains (c))) {
                            CurrenciesValues[c] += e.Value!;
                        }
                    } else if (e.TargetIDs[0] == Currency.REGION) {
                        foreach (IDType c in CurrenciesValues.Keys.Where (c => _regionsActive.Contains (c))) {
                            CurrenciesValues[c] += e.Value!;
                        }
                    } else {
                        foreach (IDType c in CurrenciesValues.Keys.Where (c => e.TargetIDs.Contains (c))) {
                            CurrenciesValues[c] += e.Value!;
                        }
                    }

                    break;
            }
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }
    }

    public void InitialisePeople (List<Person> people) {
        foreach (Person p in people) {
            _people[p.ID] = p;
            _peopleRoles[p.ID] = [Role.MEMBER];
            _peopleFactions[p.ID] = (null, null);
        }
    }

    public bool? IsBallotVoted () => Context?.IsBallotVoted ();

    public bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public sbyte GetCurrencyValue (IDType currencyId) => CurrenciesValues.GetValueOrDefault (currencyId);

    public bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
