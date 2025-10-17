using System.Diagnostics;

namespace CongressCucuta.Data;

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

internal class VotedBallotEventArgs (IDType id, bool isPassed, SimulationContext.BallotContext context) {
    public IDType ID = id;
    public byte VotesPass = context.CalculateVotesPass ();
    public byte VotesFail = context.CalculateVotesFail ();
    public byte VotesAbstain = context.CalculateVotesAbstain ();
    public bool IsPass = isPassed;
    public List<IDType> ProceduresDeclared = context.ProceduresDeclared;
}

internal class CompletedElectionEventArgs (
    Dictionary<IDType, Person> people,
    Dictionary<IDType, SortedSet<IDType>> peopleRoles,
    Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
    bool isBallot
) {
    public Dictionary<IDType, Person> People = people;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRoles = peopleRoles;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactions = peopleFactions;
    public bool IsBallot = isBallot;
}

internal class UpdatedVotesEventArgs (SimulationContext.BallotContext context) {
    public byte VotesPass = context.CalculateVotesPass ();
    public byte VotesFail = context.CalculateVotesFail ();
    public byte VotesAbstain = context.CalculateVotesAbstain ();
    public byte VotesPassThreshold = context.CalculateVotesPassThreshold ();
    public byte VotesFailThreshold = context.CalculateVotesFailThreshold ();
}

// Must call InitialisePeople () after construction to complete initialisation
internal class SimulationContext (Simulation simulation) {
    internal class BallotContext {
        // Replaced as necessary (every ballot, upon VotersLimit declared, upon role change)
        public Dictionary<IDType, Permissions> PeoplePermissions { get; set; } = [];
        public List<IDType> VotesPass { get; } = [];
        public List<IDType> VotesFail { get; } = [];
        public byte VotesPassBonus { get; set; } = 0;
        public byte VotesFailBonus { get; set; } = 0;
        public bool IsSimpleMajority { get; set; } = true;
        public List<IDType> ProceduresDeclared = [];
        public event Action<UpdatedVotesEventArgs>? UpdatedVotes;

        public void Reset () {
            VotesPass.Clear ();
            VotesFail.Clear ();
            ProceduresDeclared.Clear ();
            VotesPassBonus = 0;
            VotesFailBonus = 0;
        }

        // For ProcedureDeclared
        public void ResetVotes () {
            VotesPass.Clear ();
            VotesFail.Clear ();

            UpdatedVotesEventArgs args = new (this);

            UpdatedVotes?.Invoke (args);
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

    internal readonly record struct ConfirmationResult (
        Procedure.Confirmation.CostType Cost,
        bool? IsConfirmed,
        byte? Value = null,
        (IDType, sbyte)? Currency = null,
        int? DiceDeclarer = null,
        int? DiceDefender = null
    );

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
    private readonly Random _random = new ();
    // (person, composition)
    private readonly List<(IDType, Permissions.Composition)> _compositions = [];
    private readonly List<Procedure.Effect> _effectsPermissions = [];
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
    public bool IsBallot { get; set; }
    public event Action<Dictionary<IDType, Person>>? InitialisedPeople;
    public event Action<PreparingElectionEventArgs>? PreparingElection;
    public event Action<CompletedElectionEventArgs>? CompletedElection;
    public event Action<Dictionary<IDType, Permissions>>? UpdatedPermissions;
    public event Action<VotedBallotEventArgs>? VotedBallot;
    public event Action<Dictionary<IDType, sbyte>>? ModifiedCurrencies;
    public event Action<HashSet<ProcedureTargeted>>? ModifiedProcedures;

    private void ComposePermissions () {
        Dictionary<IDType, Permissions> peoplePermissions = [];

        foreach (var kv in PeopleRoles) {
            Permissions permissions = _rolesPermissions[kv.Value.Last ()];

            foreach (IDType r in kv.Value.Reverse ()) {
                permissions += _rolesPermissions[r];
            }

            peoplePermissions[kv.Key] = permissions;
        }

        foreach (Procedure.Effect e in _effectsPermissions) {
            switch (e.Action) {
                case Procedure.Effect.ActionType.PermissionsCanVote: {
                    bool canVote = e.Value > 0;
                    Permissions.Composition composition = new (CanVote: canVote);

                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType r in e.TargetIDs) {
                            foreach (var kv in PeopleRoles) {
                                if (kv.Value.Contains (r)) {
                                    peoplePermissions[kv.Key] += composition;
                                }
                            }
                        }
                    } else {
                        IDType personId = _random.Next (People.Count);

                        peoplePermissions[personId] += composition;
                    }

                    break;
                }
                case Procedure.Effect.ActionType.PermissionsVotes: {
                    Permissions.Composition composition = new (Votes: e.Value);

                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType r in e.TargetIDs) {
                            foreach (var kv in PeopleRoles) {
                                if (kv.Value.Contains (r)) {
                                    peoplePermissions[kv.Key] += composition;
                                }
                            }
                        }
                    } else {
                        IDType personId = _random.Next (People.Count);

                        peoplePermissions[personId] += composition;
                    }

                    break;
                }
                case Procedure.Effect.ActionType.PermissionsCanSpeak: {
                    bool canSpeak = e.Value > 0;
                    Permissions.Composition composition = new (CanSpeak: canSpeak);

                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType r in e.TargetIDs) {
                            foreach (var kv in PeopleRoles) {
                                if (kv.Value.Contains (r)) {
                                    peoplePermissions[kv.Key] += composition;
                                }
                            }
                        }
                    } else {
                        IDType personId = _random.Next (People.Count);

                        peoplePermissions[personId] += composition;
                    }

                    break;
                }
            }
        }

        foreach ((IDType personId, Permissions.Composition composition) in _compositions) {
            peoplePermissions[personId] += composition;
        }

        Context.PeoplePermissions = peoplePermissions;
        UpdatedPermissions?.Invoke (peoplePermissions);
    }

    public ConfirmationResult TryConfirmProcedure (IDType personId, IDType procedureId) {
        ProcedureDeclared procedure = ProceduresDeclared[procedureId];
        Procedure.EffectBundle effects = (Procedure.EffectBundle) procedure.YieldEffects (BallotCurrentID)!;
        Procedure.Confirmation confirmation = (Procedure.Confirmation) effects.Confirmation!;

        Context.ProceduresDeclared.Add (procedureId);

        switch (confirmation.Cost) {
            case Procedure.Confirmation.CostType.Always: {
                return new (Procedure.Confirmation.CostType.Always, true);
            }
            case Procedure.Confirmation.CostType.DivisionChamber: {
                return new (Procedure.Confirmation.CostType.DivisionChamber, null);
            }
            case Procedure.Confirmation.CostType.CurrencyValue: {
                IDType currencyId = ChooseCurrencyOwner (personId);

                if (CurrenciesValues[currencyId] >= confirmation.Value) {
                    CurrenciesValues[currencyId] -= (sbyte) confirmation.Value;
                    OnModifiedCurrencies ();
                    return new (
                        Procedure.Confirmation.CostType.CurrencyValue,
                        true,
                        Value: confirmation.Value,
                        Currency: (currencyId, CurrenciesValues[currencyId])
                    );
                } else {
                    throw new UnreachableException ();
                }
            }
            case Procedure.Confirmation.CostType.DiceValue: {
                int dice = _random.Next (1, 7);

                return new (
                    Procedure.Confirmation.CostType.DiceValue,
                    dice >= confirmation.Value,
                    Value: confirmation.Value,
                    DiceDeclarer: dice
                );
            }
            case Procedure.Confirmation.CostType.DiceCurrency: {
                IDType currencyId = ChooseCurrencyOwner (personId);
                int dice = _random.Next (1, 7);

                if (CurrenciesValues[currencyId] >= dice) {
                    CurrenciesValues[currencyId] -= (sbyte) dice;
                    OnModifiedCurrencies ();
                    return new (
                        Procedure.Confirmation.CostType.DiceCurrency,
                        true,
                        Currency: (currencyId, CurrenciesValues[currencyId]),
                        DiceDeclarer: dice
                    );
                } else {
                    return new (
                        Procedure.Confirmation.CostType.DiceCurrency,
                        false,
                        Currency: (currencyId, CurrenciesValues[currencyId]),
                        DiceDeclarer: dice
                    );
                }
            }
            case Procedure.Confirmation.CostType.DiceAdversarial: {
                int diceDeclarer = _random.Next (1, 7);
                int diceDefender = _random.Next (1, 7);
                (IDType, sbyte)? currency = null;

                if (CurrenciesValues.Count > 0) {
                    IDType currencyId = ChooseCurrencyOwner (personId);

                    currency = (currencyId, CurrenciesValues[currencyId]);

                    if (CurrenciesValues[currencyId] >= diceDeclarer) {
                        CurrenciesValues[currencyId] -= (sbyte) diceDeclarer;
                        OnModifiedCurrencies ();
                    } else {
                        return new (
                            Procedure.Confirmation.CostType.DiceAdversarial,
                            false,
                            Currency: currency,
                            DiceDeclarer: diceDeclarer
                        );
                    }
                }

                if (diceDeclarer >= diceDefender) {
                    return new (
                        Procedure.Confirmation.CostType.DiceAdversarial,
                        true,
                        Currency: currency,
                        DiceDeclarer: diceDeclarer,
                        DiceDefender: diceDefender
                    );
                } else {
                    return new (
                        Procedure.Confirmation.CostType.DiceAdversarial,
                        false,
                        Currency: currency,
                        DiceDeclarer: diceDeclarer,
                        DiceDefender: diceDefender
                    );
                }
            }
            default:
                throw new UnreachableException ();
        }
    }

    public bool? DeclareProcedure (IDType personId, IDType procedureId) {
        bool? isPass = null;

        foreach (Procedure.Effect e in ProceduresDeclared[procedureId].Effects) {
            switch (e.Action) {
                case Procedure.Effect.ActionType.CurrencyAdd: {
                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType c in e.TargetIDs) {
                            CurrenciesValues[c] += (sbyte) e.Value;
                        }
                    } else {
                        IDType currencyId = ChooseCurrencyOwner (personId);

                        CurrenciesValues[currencyId] += (sbyte) e.Value;
                    }

                    OnModifiedCurrencies ();
                    break;
                }
                case Procedure.Effect.ActionType.CurrencySubtract: {
                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType c in e.TargetIDs) {
                            CurrenciesValues[c] -= (sbyte) e.Value;
                        }
                    } else {
                        IDType currencyId = ChooseCurrencyOwner (personId);

                        CurrenciesValues[currencyId] -= (sbyte) e.Value;
                    }

                    OnModifiedCurrencies ();
                    break;
                }
                case Procedure.Effect.ActionType.ElectionRegion: {
                    Election election = new (procedureId, e);

                    OnPrepareElection ([election]);
                    Context.ResetVotes ();
                    break;
                }
                case Procedure.Effect.ActionType.ElectionParty: {
                    Election election = new (procedureId, e);

                    OnPrepareElection ([election]);
                    Context.ResetVotes ();
                    break;
                }
                case Procedure.Effect.ActionType.ElectionNominated: {
                    Election election = new (procedureId, e);

                    OnPrepareElection ([election]);
                    Context.ResetVotes ();
                    break;
                }
                case Procedure.Effect.ActionType.ElectionAppointed: {
                    Election election = new (procedureId, e);

                    OnPrepareElection ([election]);
                    Context.ResetVotes ();
                    break;
                }
                case Procedure.Effect.ActionType.BallotLimit: {
                    foreach (IDType p in People.Keys) {
                        if (
                            p == personId
                            || PeopleRoles[p].Any (r => e.TargetIDs.Contains (r))
                            || e.TargetIDs.Any (r => r == PeopleFactions[p].Item1)
                            || e.TargetIDs.Any (r => r == PeopleFactions[p].Item2)
                        ) {
                            _compositions.Add ((p, new Permissions.Composition (CanVote: true)));
                        } else {
                            _compositions.Add ((p, new Permissions.Composition (CanVote: false)));
                        }
                    }

                    ComposePermissions ();
                    Context.ResetVotes ();
                    break;
                }
                case Procedure.Effect.ActionType.BallotPass: {
                    VoteBallot (true);
                    isPass = true;
                    break;
                }
                case Procedure.Effect.ActionType.BallotFail: {
                    VoteBallot (false);
                    isPass = false;
                    break;
                }
            }
        }

        return isPass;
    }

    private void OnCompleteElection () {
        CompletedElectionEventArgs args = new (People, PeopleRoles, PeopleFactions, IsBallot);

        CompletedElection?.Invoke (args);
        ComposePermissions ();
    }

    private void OnPrepareElection (List<Election> elections) {
        PreparingElectionEventArgs args = new (elections, in PeopleRoles, in PeopleFactions, in _partiesActive, in _regionsActive, in _people);

        PreparingElection?.Invoke (args);
        PeopleRoles = args.PeopleRolesNew;
        PeopleFactions = args.PeopleFactionsNew;
        OnCompleteElection ();
    }

    private void OnModifiedCurrencies () => ModifiedCurrencies?.Invoke (CurrenciesValues);

    private void OnModifiedProcedures () {
        HashSet<ProcedureTargeted> procedures = [];

        foreach (ProcedureTargeted p in ProceduresSpecial.Values) {
            if (_proceduresActive.Contains (p.ID)) {
                procedures.Add (p);
            }
        }

        ModifiedProcedures?.Invoke (procedures);
    }

    public void StartSetup () {
        List<(IDType, Procedure.EffectBundle)> effects = [
            .. ProceduresGovernmental.Values.Where (pi => pi.IsActiveStart)
                .Select (pi => (pi.ID, (Procedure.EffectBundle) pi.YieldEffects (0)!))
        ];
        List<Election> elections = [];

        foreach ((IDType p, Procedure.EffectBundle eb) in effects) {
            foreach (Procedure.Effect e in eb.Effects) {
                switch (e.Action) {
                    case Procedure.Effect.ActionType.ElectionRegion:
                    case Procedure.Effect.ActionType.ElectionParty:
                    case Procedure.Effect.ActionType.ElectionNominated:
                    case Procedure.Effect.ActionType.ElectionAppointed:
                        elections.Add (new Election (p, e));
                        break;
                    case Procedure.Effect.ActionType.PermissionsCanVote: {
                        bool canVote = e.Value > 0;
                        Permissions.Composition composition = new (CanVote: canVote);

                        foreach (IDType r in e.TargetIDs) {
                            _rolesPermissions[r] += composition;
                        }

                        break;
                    }
                    case Procedure.Effect.ActionType.PermissionsVotes: {
                        Permissions.Composition composition = new (Votes: e.Value);

                        foreach (IDType r in e.TargetIDs) {
                            _rolesPermissions[r] += composition;
                        }

                        break;
                    }
                    case Procedure.Effect.ActionType.PermissionsCanSpeak: {
                        bool canSpeak = e.Value > 0;
                        Permissions.Composition composition = new (CanSpeak: canSpeak);

                        foreach (IDType r in e.TargetIDs) {
                            _rolesPermissions[r] += composition;
                        }

                        break;
                    }
                }
            }
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }

        if (CurrenciesValues.Count > 0) {
            OnModifiedCurrencies ();
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
        _effectsPermissions.Clear ();
        _compositions.Clear ();

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
                    case Procedure.Effect.ActionType.ProcedureActivate: {
                        // Don't apply on first ballot, since elections were already held
                        if (BallotCurrentID > 0) {
                            foreach (IDType p in e.TargetIDs) {
                                foreach (Procedure.Effect ee in ProceduresGovernmental[p].Effects) {
                                    if (
                                        ee.Action is Procedure.Effect.ActionType.ElectionRegion
                                        or Procedure.Effect.ActionType.ElectionParty
                                        or Procedure.Effect.ActionType.ElectionNominated
                                        or Procedure.Effect.ActionType.ElectionAppointed
                                    ) {
                                        effectsElections.Add ((p, ee));
                                    }
                                } 
                            }
                        }

                        break;
                    }
                    case Procedure.Effect.ActionType.PermissionsCanVote:
                        _effectsPermissions.Add (e);
                        break;
                    case Procedure.Effect.ActionType.PermissionsVotes:
                        _effectsPermissions.Add (e);
                        break;
                    case Procedure.Effect.ActionType.PermissionsCanSpeak:
                        _effectsPermissions.Add (e);
                        break;
                }
            }
        }

        if (isModifiedCurrencies) {
            OnModifiedCurrencies ();
        }

        if (effectsElections.Count > 0) {
            List<Election> elections = effectsElections.ConvertAll (e => new Election (e.Item1, e.Item2));

            OnPrepareElection (elections);
        }

        if (_effectsPermissions.Count > 0) {
            ComposePermissions ();
        }
    }

    private void EndBallot (bool isPass) {
        List<Ballot.Effect> effects;
        List<Election> elections = [];
        bool isModifiedCurrencies = false;
        bool isModifiedProcedures = false;

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
                //case Ballot.Effect.ActionType.ReplaceParty:
                //    IDType partyOriginal = e.TargetIDs[0];
                //    IDType partyNew = e.TargetIDs[1];

                //    _partiesActive.Remove (partyOriginal);
                //    _partiesActive.Add (partyNew);

                //    foreach (var kv in PeopleFactions) {
                //        if (kv.Value.Item1 is IDType p && p == partyOriginal) {
                //            PeopleFactions[kv.Key] = (partyNew, kv.Value.Item2);
                //        }
                //    }

                //    foreach (var kv in PeopleRoles) {
                //        if (kv.Value.Remove (partyOriginal)) {
                //            kv.Value.Add (partyNew);
                //        }
                //    }

                //    // update currencies, which seems impossible to maintain consistently

                //    OnCompleteElection ();
                //    break;
                case Ballot.Effect.ActionType.RemoveProcedure:
                    _proceduresActive.RemoveWhere (p => e.TargetIDs.Contains (p));
                    isModifiedProcedures = true;
                    break;
                case Ballot.Effect.ActionType.ReplaceProcedure:
                    _proceduresActive.Remove (e.TargetIDs[0]);
                    _proceduresActive.Add (e.TargetIDs[1]);
                    isModifiedProcedures = true;
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
            OnModifiedCurrencies ();
        }

        if (isModifiedProcedures) {
            OnModifiedProcedures ();
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }
    }

    public IDType ChooseCurrencyOwner (IDType personId) {
        if (PeopleFactions[personId].Item1 is IDType p && Currencies.Contains (p)) {
            return p;
        } else if (PeopleFactions[personId].Item2 is IDType r && Currencies.Contains (r)) {
            return r;
        }

        return Currency.STATE;
    }

    public (byte, byte, byte) VotePass (IDType personId, bool isPass) {
        if (isPass) {
            Context.VotesPass.Add (personId);
        } else {
            Context.VotesPass.Remove (personId);
        }

        return (Context.CalculateVotesPass (), Context.CalculateVotesFail (), Context.CalculateVotesAbstain ());
    }

    public (byte, byte, byte) VoteFail (IDType personId, bool isFail) {
        if (isFail) {
            Context.VotesFail.Add (personId);
        } else {
            Context.VotesFail.Remove (personId);
        }

        return (Context.CalculateVotesPass (), Context.CalculateVotesFail (), Context.CalculateVotesAbstain ());
    }

    public (byte, byte, byte) VoteAbstain (IDType personId, bool isAbstain) {
        if (isAbstain) {
            Context.VotesPass.Remove (personId);
            Context.VotesFail.Remove (personId);
        }

        return (Context.CalculateVotesPass (), Context.CalculateVotesFail (), Context.CalculateVotesAbstain ());
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

        VotedBallotEventArgs args = new (_ballotCurrentId, isPass, Context);

        VotedBallot?.Invoke (args);
        EndBallot (isPass);
    }

    public bool? IsBallotVoted () => Context.IsBallotVoted ();

    public bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public sbyte GetCurrencyValue (IDType currencyId) => CurrenciesValues.GetValueOrDefault (currencyId);

    public bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
