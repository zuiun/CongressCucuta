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

internal class VotedBallotEventArgs (IDType id, SimulationContext.BallotContext context) {
    public IDType ID = id;
    public byte VotesPass = context.CalculateVotesPass ();
    public byte VotesFail = context.CalculateVotesFail ();
    public byte VotesAbstain = context.CalculateVotesAbstain ();
}

internal class CompletedElectionEventArgs (
    Dictionary<IDType, Person> people,
    Dictionary<IDType, SortedSet<IDType>> peopleRoles,
    Dictionary<IDType, (IDType?, IDType?)> peopleFactions
) {
    public Dictionary<IDType, Person> People = people;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRoles = peopleRoles;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactions = peopleFactions;
}

// Must call InitialisePeople () after construction to complete initialisation
internal class SimulationContext (Simulation simulation) {
    // TODO: not sure that this is necessary. it could definitely be done in the parent, though it would be very annoying
    internal class BallotContext {
        // Replaced as necessary (every ballot, upon VotersLimit declared, upon role change)
        public Dictionary<IDType, Permissions> PeoplePermissions { get; set; } = [];
        public HashSet<IDType> VotesPass { get; } = [];
        public HashSet<IDType> VotesFail { get; } = [];
        public byte VotesPassBonus { get; set; } = 0;
        public byte VotesFailBonus { get; set; } = 0;
        public bool IsSimpleMajority { get; set; } = true;

        public void Reset () {
            VotesPass.Clear ();
            VotesFail.Clear ();
            VotesPassBonus = 0;
            VotesFailBonus = 0;
        }

        public byte CalculateVotesTotal () {
            byte votes = (byte) (VotesPassBonus + VotesFailBonus);

            foreach (Permissions p in PeoplePermissions.Values) {
                if (p.CanVote) {
                    votes += p.Votes;
                }
            }

            return votes;
        }

        public byte CalculateVotesPass () {
            byte passCount = VotesPassBonus;

            foreach (IDType p in VotesPass) {
                passCount += PeoplePermissions[p].Votes;
            }

            return passCount;
        }

        public byte CalculateVotesFail () {
            byte failCount = VotesFailBonus;

            foreach (IDType p in VotesFail) {
                failCount += PeoplePermissions[p].Votes;
            }

            return failCount;
        }

        public byte CalculateVotesAbstain () => (byte) (CalculateVotesTotal () - CalculateVotesPass () - CalculateVotesFail ());

        public byte CalculateVotesPassThreshold () {
            byte votesTotal = CalculateVotesTotal ();

            return IsSimpleMajority
                ? (byte) (Math.Floor (votesTotal / 2m) + 1)
                : (byte) Math.Ceiling ((votesTotal * 2) / 3m);
        }

        public byte CalculateVotesFailThreshold () {
            byte votesTotal = CalculateVotesTotal ();
            byte votesPassThreshold = CalculateVotesPassThreshold ();

            return IsSimpleMajority
                ? votesPassThreshold
                : (byte) (votesTotal - votesPassThreshold + 1);
        }

        public bool? IsBallotVoted () {
            byte votesPassThreshold = CalculateVotesPassThreshold ();
            byte votesFailThreshold = CalculateVotesFailThreshold ();
            byte votesPass = CalculateVotesPass ();
            byte votesFail = CalculateVotesFail ();

            if (votesPass >= votesPassThreshold) {
                return true;
            } else if (votesFail >= votesFailThreshold) {
                return false;
            } else {
                return null;
            }
        }
    }

    private readonly Dictionary<IDType, Person> _people = [];
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
    public readonly BallotContext Context = new ();
    public IDType BallotCurrentID {
        get => _ballotCurrentId;
        set {
            _ballotCurrentId = value;
            StartBallot ();
        }
    }
    public Dictionary<IDType, Person> People => _people;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRoles = [];
    // (party, region)
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactions = [];
    public HashSet<IDType> Roles { get; } = [.. simulation.RolesPermissions.Keys];
    public Dictionary<IDType, Faction> Parties { get; } = simulation.Parties.ToDictionary (p => p.ID, p => p);
    public Dictionary<IDType, Faction> Regions { get; } = simulation.Regions.ToDictionary (r => r.ID, r => r);
    public HashSet<IDType> Currencies { get; } = [.. simulation.CurrenciesValues.Keys];
    public Dictionary<IDType, sbyte> CurrenciesValues { get; } = simulation.CurrenciesValues;
    public Dictionary<IDType, ProcedureImmediate> ProceduresGovernmental { get; } = simulation.ProceduresGovernmental.ToDictionary (pi => pi.ID, pi => pi);
    public Dictionary<IDType, ProcedureTargeted> ProceduresSpecial { get; } = simulation.ProceduresSpecial.ToDictionary (pt => pt.ID, pt => pt);
    public Dictionary<IDType, ProcedureDeclared> ProceduresDeclared { get; } = simulation.ProceduresDeclared.ToDictionary (pd => pd.ID, pd => pd);
    public Dictionary<IDType, Ballot> Ballots { get; } = simulation.Ballots.ToDictionary (b => b.ID, b => b);
    public event Action<Dictionary<IDType, Person>>? InitialisedPeople;
    public event Action<PreparingElectionEventArgs>? PreparingElection;
    public event Action<CompletedElectionEventArgs>? CompletedElection;
    public event Action<Dictionary<IDType, Permissions>>? UpdatedPermissions;
    public event Action<VotedBallotEventArgs>? VotedBallot;
    public event Action<Dictionary<IDType, sbyte>>? ModifiedCurrencies;

    private void ComposePermissions () {
        Dictionary<IDType, Permissions> peoplePermissions = [];

        foreach (var kv in PeopleRoles) {
            Permissions permissions = _rolesPermissions[kv.Value.Last ()];

            foreach (IDType r in kv.Value.Reverse ()) {
                permissions += _rolesPermissions[r];
            }

            peoplePermissions[kv.Key] = permissions;
        }

        Context.PeoplePermissions = peoplePermissions;
        UpdatedPermissions?.Invoke (peoplePermissions);
    }

    private void LimitVoters (List<IDType> allowedRoleIds) {
        foreach (IDType p in Context.PeoplePermissions.Keys) {
            if (PeopleRoles[p].All (r => ! allowedRoleIds.Contains (r))) {
                Context.PeoplePermissions[p] += new Permissions.Composition (CanVote: false);
            }
        }

        UpdatedPermissions?.Invoke (Context.PeoplePermissions);
    }

    private void OnPrepareElection (List<Election> elections) {
        PreparingElectionEventArgs argsPreparing = new (elections, in PeopleRoles, in PeopleFactions, in _partiesActive, in _regionsActive, in _people);

        PreparingElection?.Invoke (argsPreparing);
        PeopleRoles = argsPreparing.PeopleRolesNew;
        PeopleFactions = argsPreparing.PeopleFactionsNew;

        CompletedElectionEventArgs argsCompleted = new (_people, PeopleRoles, PeopleFactions);

        CompletedElection?.Invoke (argsCompleted);
        ComposePermissions ();
    }

    public void StartSetup () {
        List<(IDType, Procedure.EffectBundle)> effects = [
            .. ProceduresGovernmental.Values.Where (pi => pi.IsActiveStart)
                .Select (pi => (pi.ID, (Procedure.EffectBundle) pi.YieldEffects (0)!))
        ];
        List<Election> elections = effects.ConvertAll (e => new Election (e.Item1, e.Item2.Effects[0], true));

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }

        if (CurrenciesValues.Count > 0) {
            ModifiedCurrencies?.Invoke (CurrenciesValues);
        }
    }

    private void StartBallot () {
        // These are guaranteed to be ProcedureTargeted
        var effects = _proceduresActive.Select (pt => ProceduresSpecial[pt])
            .Select (pt => pt.YieldEffects (BallotCurrentID))
            .Where (e => e is not null)
            .Select (e => (Procedure.EffectBundle) e!);
        List<(IDType, Procedure.Effect)> effectsElections = [];
        bool isModifiedCurrencies = false;

        Context.Reset ();

        foreach (Procedure.EffectBundle eb in effects) {
            foreach (Procedure.Effect e in eb.Effects) {
                switch (e.Action) {
                    case Procedure.Effect.ActionType.VotePassAdd:
                        Context.VotesPassBonus += e.Value;
                        break;
                    case Procedure.Effect.ActionType.VoteFailAdd:
                        Context.VotesFailBonus += e.Value;
                        break;
                    case Procedure.Effect.ActionType.VotePassTwoThirds:
                        Context.IsSimpleMajority = false;
                        break;
                    case Procedure.Effect.ActionType.CurrencyAdd:
                        // Don't apply on the first ballot, since it's already pre-initialised
                        if (BallotCurrentID > 0) {
                            if (e.TargetIDs.Length > 0) {
                                foreach (IDType c in CurrenciesValues.Keys.Where (c => e.TargetIDs.Contains (c))) {
                                    CurrenciesValues[c] += (sbyte) e.Value;
                                }
                            } else {
                                CurrenciesValues[Currency.STATE] += (sbyte) e.Value;

                            }

                            isModifiedCurrencies = true;
                        }
                        break;
                    case Procedure.Effect.ActionType.CurrencySubtract:
                        // Don't apply on the first ballot, since it's already pre-initialised
                        if (BallotCurrentID > 0) {
                            if (e.TargetIDs.Length > 0) {
                                foreach (IDType c in CurrenciesValues.Keys.Where (c => e.TargetIDs.Contains (c))) {
                                    CurrenciesValues[c] -= (sbyte) e.Value;
                                }
                            } else {
                                CurrenciesValues[Currency.STATE] -= (sbyte) e.Value;

                            }

                            isModifiedCurrencies = true;
                        }
                        break;
                    case Procedure.Effect.ActionType.ProcedureActivate:
                        effectsElections.Add ((e.TargetIDs[0], e));
                        break;
                }
            }
        }

        if (isModifiedCurrencies) {
            ModifiedCurrencies?.Invoke (CurrenciesValues);
        }

        if (effectsElections.Count > 0) {
            List<Election> elections = effectsElections.ConvertAll (e => new Election (e.Item1, e.Item2, false));

            OnPrepareElection (elections);
        }

        // TODO: another event that tells context how many votes are needed to pass
        // TODO: every vote should also result in a return event that tells how many votes there are right now
    }

    private void EndBallot (bool isPass) {
        List<Ballot.Effect> effects;
        List<Election> elections = [];
        bool isModifiedCurrencies = false;

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

                    isModifiedCurrencies = true;
                    break;
            }
        }

        if (isModifiedCurrencies) {
            ModifiedCurrencies?.Invoke (CurrenciesValues);
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }
    }

    public byte VotePass (IDType personId, bool isPass) {
        if (isPass) {
            Context.VotesPass.Add (personId);
        } else {
            Context.VotesPass.Remove (personId);
        }

        return Context.CalculateVotesPass ();
    }

    public byte VoteFail (IDType personId, bool isFail) {
        if (isFail) {
            Context.VotesFail.Add (personId);
        } else {
            Context.VotesFail.Remove (personId);
        }

        return Context.CalculateVotesFail ();
    }

    public byte VoteAbstain (IDType personId, bool isAbstain) {
        if (isAbstain) {
            Context.VotesPass.Remove (personId);
            Context.VotesFail.Remove (personId);
        }

        return Context.CalculateVotesAbstain ();
    }

    public void InitialisePeople (List<Person> people) {
        foreach (Person p in people) {
            _people[p.ID] = p;
            PeopleRoles[p.ID] = [Role.MEMBER];
            PeopleFactions[p.ID] = (null, null);
        }

        InitialisedPeople?.Invoke (_people);
    }

    public void VoteBallot (bool isPass) {
        if (isPass) {
            _ballotsPassed.Add (_ballotCurrentId);
        }

        VotedBallotEventArgs args = new (_ballotCurrentId, Context);

        VotedBallot?.Invoke (args);
        EndBallot (isPass);
    }

    public bool? IsBallotVoted () => Context.IsBallotVoted ();

    public bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public sbyte GetCurrencyValue (IDType currencyId) => CurrenciesValues.GetValueOrDefault (currencyId);

    public bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
