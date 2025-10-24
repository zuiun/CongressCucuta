using System.Diagnostics;
using CongressCucuta.Core.Generators;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Core.Contexts;

public class PreparingElectionEventArgs (
    List<ElectionContext> elections,
    Dictionary<IDType, SortedSet<IDType>> peopleRoles,
    Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
    HashSet<IDType> partiesActive,
    HashSet<IDType> regionsActive,
    Dictionary<IDType, Person> people
) {
    public List<ElectionContext> Elections = elections;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRoles = peopleRoles;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactions = peopleFactions;
    public HashSet<IDType> PartiesActive = partiesActive;
    public HashSet<IDType> RegionsActive = regionsActive;
    public Dictionary<IDType, Person> People = people;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRolesNew = [];
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactionsNew = [];
}

public class VotedBallotEventArgs (IDType id, bool isPassed, BallotContext context) {
    public IDType ID = id;
    public byte VotesPass = context.CalculateVotesPass ();
    public byte VotesFail = context.CalculateVotesFail ();
    public byte VotesAbstain = context.CalculateVotesAbstain ();
    public bool IsPassed = isPassed;
    public List<IDType> ProceduresDeclared = context.ProceduresDeclared;
}

public class CompletedElectionEventArgs (
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

// Must call InitialisePeople () after construction to complete initialisation
public class SimulationContext (Simulation simulation, IGenerator? generator = null) {
    public readonly record struct ConfirmationResult (
        Confirmation.ConfirmationType Type,
        bool? IsConfirmed,
        byte? Value = null,
        (IDType, sbyte)? Currency = null,
        int? DiceDeclarer = null,
        int? DiceDefender = null
    );

    private readonly IGenerator _generator = generator ?? new RandomGenerator ();
    private readonly Dictionary<IDType, Person> _people = [];
    private readonly Dictionary<IDType, Permissions> _rolesPermissions = simulation.RolesPermissions.ToDictionary (k => k.Key, k => k.Value);
    private readonly HashSet<IDType> _partiesActive = [.. simulation.Parties.Where (p => p.IsActiveStart).Select (p => p.ID)];
    private readonly HashSet<IDType> _regionsActive = [.. simulation.Regions.Where (r => r.IsActiveStart).Select (r => r.ID)];
    private readonly HashSet<IDType> _proceduresActive = [.. simulation.ProceduresSpecial.Where (pt => pt.IsActiveStart).Select (pt => pt.ID)];
    private readonly HashSet<IDType> _ballotsPassed = [];
    // (person, composition)
    private readonly List<(IDType, Permissions.Composition)> _compositions = [];
    private readonly List<Procedure.Effect> _effectsPermissions = [];
    public readonly BallotContext Context = new ();
    public IDType BallotCurrentID { get; set; } = 0;
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
    public event Action<PreparingElectionEventArgs>? PreparingElection;
    public event Action<CompletedElectionEventArgs>? CompletedElection;
    public event Action<Dictionary<IDType, Permissions>>? UpdatedPermissions;
    public event Action<VotedBallotEventArgs>? VotedBallot;
    public event Action<Dictionary<IDType, sbyte>>? ModifiedCurrencies;
    public event Action<HashSet<ProcedureTargeted>>? ModifiedProcedures;

    public void InitialisePeople (List<Person> people) {
        foreach (Person p in people) {
            _people[p.ID] = p;
            PeopleRoles[p.ID] = [Role.MEMBER];
            PeopleFactions[p.ID] = (null, null);
        }
    }

    private void OnPrepareElection (List<ElectionContext> elections) {
        PreparingElectionEventArgs argsPrepare = new (elections, PeopleRoles, PeopleFactions, _partiesActive, _regionsActive, _people);

        PreparingElection?.Invoke (argsPrepare);
        PeopleRoles = argsPrepare.PeopleRolesNew;
        PeopleFactions = argsPrepare.PeopleFactionsNew;

        CompletedElectionEventArgs argsComplete = new (_people, PeopleRoles, PeopleFactions, IsBallot);

        CompletedElection?.Invoke (argsComplete);
        ComposePermissions ();
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
            switch (e.Type) {
                case Procedure.Effect.EffectType.PermissionsCanVote: {
                    bool canVote = e.Value > 0;
                    Permissions.Composition composition = new (CanVote: canVote);

                    if (e.TargetIDs.Length > 0) {
                        foreach (var kv in PeopleRoles) {
                            if (kv.Value.Intersect (e.TargetIDs).Any ()) {
                                peoplePermissions[kv.Key] += composition;
                            }
                        }
                    } else {
                        IDType personId = _generator.Choose (_people.Count);

                        peoplePermissions[personId] += composition;
                    }

                    break;
                }
                case Procedure.Effect.EffectType.PermissionsVotes: {
                    Permissions.Composition composition = new (Votes: e.Value);

                    if (e.TargetIDs.Length > 0) {
                        foreach (var kv in PeopleRoles) {
                            if (kv.Value.Intersect (e.TargetIDs).Any ()) {
                                peoplePermissions[kv.Key] += composition;
                            }
                        }
                    } else {
                        IDType personId = _generator.Choose (_people.Count);

                        peoplePermissions[personId] += composition;
                    }

                    break;
                }
                case Procedure.Effect.EffectType.PermissionsCanSpeak: {
                    bool canSpeak = e.Value > 0;
                    Permissions.Composition composition = new (CanSpeak: canSpeak);

                    if (e.TargetIDs.Length > 0) {
                        foreach (var kv in PeopleRoles) {
                            if (kv.Value.Intersect (e.TargetIDs).Any ()) {
                                peoplePermissions[kv.Key] += composition;
                            }
                        }
                    } else {
                        IDType personId = _generator.Choose (_people.Count);

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

    public IDType ChooseCurrencyOwner (IDType personId) {
        if (PeopleFactions[personId].Item1 is IDType p && Currencies.Contains (p)) {
            return p;
        } else if (PeopleFactions[personId].Item2 is IDType r && Currencies.Contains (r)) {
            return r;
        }

        return Currency.STATE;
    }

    public ConfirmationResult TryConfirmProcedure (IDType personId, IDType procedureId) {
        ProcedureDeclared procedure = ProceduresDeclared[procedureId];
        Procedure.EffectBundle effects = (Procedure.EffectBundle) procedure.YieldEffects (BallotCurrentID)!;
        Confirmation confirmation = (Confirmation) effects.Confirmation!;

        Context.ProceduresDeclared.Add (procedureId);

        switch (confirmation.Type) {
            case Confirmation.ConfirmationType.Always: {
                return new (Confirmation.ConfirmationType.Always, true);
            }
            case Confirmation.ConfirmationType.DivisionChamber: {
                return new (Confirmation.ConfirmationType.DivisionChamber, null);
            }
            case Confirmation.ConfirmationType.CurrencyValue: {
                IDType currencyId = ChooseCurrencyOwner (personId);

                if (CurrenciesValues[currencyId] >= confirmation.Value) {
                    CurrenciesValues[currencyId] -= (sbyte) confirmation.Value;
                    OnModifiedCurrencies ();
                    return new (
                        Confirmation.ConfirmationType.CurrencyValue,
                        true,
                        Value: confirmation.Value,
                        Currency: (currencyId, CurrenciesValues[currencyId])
                    );
                } else {
                    throw new UnreachableException ();
                }
            }
            case Confirmation.ConfirmationType.DiceValue: {
                int dice = _generator.Roll ();

                return new (
                    Confirmation.ConfirmationType.DiceValue,
                    dice >= confirmation.Value,
                    Value: confirmation.Value,
                    DiceDeclarer: dice
                );
            }
            case Confirmation.ConfirmationType.DiceCurrency: {
                IDType currencyId = ChooseCurrencyOwner (personId);
                int dice = _generator.Roll ();

                if (CurrenciesValues[currencyId] >= dice) {
                    CurrenciesValues[currencyId] -= (sbyte) dice;
                    OnModifiedCurrencies ();
                    return new (
                        Confirmation.ConfirmationType.DiceCurrency,
                        true,
                        Currency: (currencyId, CurrenciesValues[currencyId]),
                        DiceDeclarer: dice
                    );
                } else {
                    return new (
                        Confirmation.ConfirmationType.DiceCurrency,
                        false,
                        Currency: (currencyId, CurrenciesValues[currencyId]),
                        DiceDeclarer: dice
                    );
                }
            }
            case Confirmation.ConfirmationType.DiceAdversarial: {
                int diceDeclarer = _generator.Roll ();
                int diceDefender = _generator.Roll ();
                (IDType, sbyte)? currency = null;

                if (CurrenciesValues.Count > 0) {
                    IDType currencyId = ChooseCurrencyOwner (personId);

                    currency = (currencyId, CurrenciesValues[currencyId]);

                    if (CurrenciesValues[currencyId] >= diceDeclarer) {
                        CurrenciesValues[currencyId] -= (sbyte) diceDeclarer;
                        currency = (currencyId, CurrenciesValues[currencyId]);
                        OnModifiedCurrencies ();
                    } else {
                        return new (
                            Confirmation.ConfirmationType.DiceAdversarial,
                            false,
                            Currency: currency,
                            DiceDeclarer: diceDeclarer
                        );
                    }
                }

                return new (
                    Confirmation.ConfirmationType.DiceAdversarial,
                    diceDeclarer >= diceDefender,
                    Currency: currency,
                    DiceDeclarer: diceDeclarer,
                    DiceDefender: diceDefender
                );
            }
            default:
                throw new UnreachableException ();
        }
    }

    public bool? DeclareProcedure (IDType personId, IDType procedureId) {
        bool? isPass = null;
        List<ElectionContext> elections = [];

        foreach (Procedure.Effect e in ProceduresDeclared[procedureId].Effects) {
            switch (e.Type) {
                case Procedure.Effect.EffectType.CurrencyAdd: {
                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType c in e.TargetIDs) {
                            CurrenciesValues[c] += (sbyte) e.Value;
                        }
                    } else {
                        IDType currencyId = ChooseCurrencyOwner (personId);

                        CurrenciesValues[currencyId] += (sbyte) e.Value;
                    }

                    break;
                }
                case Procedure.Effect.EffectType.CurrencySubtract: {
                    if (e.TargetIDs.Length > 0) {
                        foreach (IDType c in e.TargetIDs) {
                            CurrenciesValues[c] -= (sbyte) e.Value;
                        }
                    } else {
                        IDType currencyId = ChooseCurrencyOwner (personId);

                        CurrenciesValues[currencyId] -= (sbyte) e.Value;
                    }

                    break;
                }
                case Procedure.Effect.EffectType.ElectionRegion: {
                    bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_REGION);

                    elections.Add (new (procedureId, e, isLeaderNeeded, generator: _generator));
                    break;
                }
                case Procedure.Effect.EffectType.ElectionParty: {
                    bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_PARTY);

                    elections.Add (new (procedureId, e, isLeaderNeeded, generator: _generator));
                    break;
                }
                case Procedure.Effect.EffectType.ElectionNominated:
                case Procedure.Effect.EffectType.ElectionAppointed:
                    elections.Add (new (procedureId, e, generator: _generator));
                    break;
                case Procedure.Effect.EffectType.BallotLimit: {
                    foreach (IDType p in _people.Keys) {
                        if (
                            p == personId
                            || PeopleRoles[p].Intersect (e.TargetIDs).Any ()
                            || e.TargetIDs.Any (r => r == PeopleFactions[p].Item1)
                            || e.TargetIDs.Any (r => r == PeopleFactions[p].Item2)
                        ) {
                            _compositions.Add ((p, new Permissions.Composition (CanVote: true)));
                        } else {
                            _compositions.Add ((p, new Permissions.Composition (CanVote: false)));
                        }
                    }

                    ComposePermissions ();
                    Context.OnResetVotes ();
                    break;
                }
                case Procedure.Effect.EffectType.BallotPass:
                    isPass = true;
                    break;
                case Procedure.Effect.EffectType.BallotFail:
                    isPass = false;
                    break;
            }
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
            Context.OnResetVotes ();
        }

        if (CurrenciesValues.Count > 0) {
            OnModifiedCurrencies ();
        }

        return isPass;
    }

    public void StartSetup () {
        List<(IDType, Procedure.EffectBundle)> effects = [
            .. ProceduresGovernmental.Values.Select (pi => (pi.ID, (Procedure.EffectBundle) pi.YieldEffects (0)!))
        ];
        List<ElectionContext> elections = [];

        foreach ((IDType p, Procedure.EffectBundle eb) in effects) {
            foreach (Procedure.Effect e in eb.Effects) {
                switch (e.Type) {
                    case Procedure.Effect.EffectType.ElectionRegion: {
                        bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_REGION);

                        elections.Add (new ElectionContext (p, e, isLeaderNeeded, generator: _generator));
                        break;
                    }
                    case Procedure.Effect.EffectType.ElectionParty: {
                        bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_PARTY);

                        elections.Add (new ElectionContext (p, e, isLeaderNeeded, generator: _generator));
                        break;
                    }
                    case Procedure.Effect.EffectType.ElectionNominated:
                    case Procedure.Effect.EffectType.ElectionAppointed:
                        elections.Add (new ElectionContext (p, e, generator: _generator));
                        break;
                    case Procedure.Effect.EffectType.PermissionsCanVote: {
                        bool canVote = e.Value > 0;
                        Permissions.Composition composition = new (CanVote: canVote);

                        foreach (IDType r in e.TargetIDs) {
                            _rolesPermissions[r] += composition;
                        }

                        break;
                    }
                    case Procedure.Effect.EffectType.PermissionsVotes: {
                        Permissions.Composition composition = new (Votes: e.Value);

                        foreach (IDType r in e.TargetIDs) {
                            _rolesPermissions[r] += composition;
                        }

                        break;
                    }
                    case Procedure.Effect.EffectType.PermissionsCanSpeak: {
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

        ComposePermissions ();
    }

    public void StartBallot () {
        // These are guaranteed to be ProcedureTargeted
        var effects = _proceduresActive.Select (pt => ProceduresSpecial[pt])
            .Select (pt => pt.YieldEffects (BallotCurrentID))
            .Where (e => e is not null)
            .Select (e => (Procedure.EffectBundle) e!);
        List<ElectionContext> elections = [];

        Context.Reset ();
        _effectsPermissions.Clear ();
        _compositions.Clear ();

        foreach (Procedure.EffectBundle eb in effects) {
            foreach (Procedure.Effect e in eb.Effects) {
                switch (e.Type) {
                    case Procedure.Effect.EffectType.VotePassAdd:
                        Context.VotesPassBonus += e.Value;
                        break;
                    case Procedure.Effect.EffectType.VoteFailAdd:
                        Context.VotesFailBonus += e.Value;
                        break;
                    case Procedure.Effect.EffectType.VotePassTwoThirds:
                        Context.IsSimpleMajority = false;
                        break;
                    case Procedure.Effect.EffectType.CurrencyAdd:
                        // Don't apply on the first ballot, since it's already pre-initialised
                        if (BallotCurrentID > 0) {
                            if (e.TargetIDs.Length > 0) {
                                foreach (IDType c in CurrenciesValues.Keys.Where (c => e.TargetIDs.Contains (c))) {
                                    CurrenciesValues[c] += (sbyte) e.Value;
                                }
                            } else {
                                CurrenciesValues[Currency.STATE] += (sbyte) e.Value;
                            }
                        }

                        break;
                    case Procedure.Effect.EffectType.CurrencySubtract:
                        // Don't apply on the first ballot, since it's already pre-initialised
                        if (BallotCurrentID > 0) {
                            if (e.TargetIDs.Length > 0) {
                                foreach (IDType c in CurrenciesValues.Keys.Where (c => e.TargetIDs.Contains (c))) {
                                    CurrenciesValues[c] -= (sbyte) e.Value;
                                }
                            } else {
                                CurrenciesValues[Currency.STATE] -= (sbyte) e.Value;
                            }
                        }

                        break;
                    case Procedure.Effect.EffectType.ProcedureActivate: {
                        // Don't apply on first ballot, since elections were already held
                        if (BallotCurrentID > 0) {
                            foreach (IDType p in e.TargetIDs) {
                                foreach (Procedure.Effect ee in ProceduresGovernmental[p].Effects) {
                                    if (ee.Type is Procedure.Effect.EffectType.ElectionRegion) {
                                        bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_REGION);

                                        elections.Add (new (p, ee, isLeaderNeeded, generator: _generator));
                                    } else if (ee.Type is Procedure.Effect.EffectType.ElectionParty) {
                                        bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_PARTY);

                                        elections.Add (new (p, ee, isLeaderNeeded, generator: _generator));
                                    } else if (
                                        ee.Type is Procedure.Effect.EffectType.ElectionNominated
                                        or Procedure.Effect.EffectType.ElectionAppointed
                                    ) {
                                        elections.Add (new (p, ee, generator: _generator));
                                    }
                                } 
                            }
                        }

                        break;
                    }
                    case Procedure.Effect.EffectType.PermissionsCanVote:
                        _effectsPermissions.Add (e);
                        break;
                    case Procedure.Effect.EffectType.PermissionsVotes:
                        _effectsPermissions.Add (e);
                        break;
                    case Procedure.Effect.EffectType.PermissionsCanSpeak:
                        _effectsPermissions.Add (e);
                        break;
                }
            }
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }

        if (CurrenciesValues.Count > 0) {
            OnModifiedCurrencies ();
        }

        if (_effectsPermissions.Count > 0) {
            ComposePermissions ();
        }
    }

    public void EndBallot (bool isPass) {
        List<Ballot.Effect> effects;
        List<ElectionContext> elections = [];
        bool isModifiedProcedures = false;

        if (isPass) {
            effects = Ballots[BallotCurrentID].Pass.Effects;
            _ballotsPassed.Add (BallotCurrentID);
        } else {
            effects = Ballots[BallotCurrentID].Fail.Effects;
        }

        foreach (Ballot.Effect e in effects) {
            switch (e.Type) {
                case Ballot.Effect.EffectType.FoundParty:
                    bool isLeaderNeeded = _rolesPermissions.ContainsKey (Role.LEADER_PARTY);

                    foreach (IDType p in e.TargetIDs) {
                        _partiesActive.Add (p);
                    }

                    elections.Add (new (ElectionContext.ElectionType.ShuffleAdd, e.TargetIDs, e.Value, isLeaderNeeded, generator: _generator));
                    break;
                case Ballot.Effect.EffectType.DissolveParty:
                    foreach (IDType p in e.TargetIDs) {
                        _partiesActive.Remove (p);
                    }

                    elections.Add (new (ElectionContext.ElectionType.ShuffleRemove, e.TargetIDs, generator: _generator));
                    break;
                //case Ballot.Effect.EffectType.ReplaceParty:
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
                case Ballot.Effect.EffectType.RemoveProcedure:
                    _proceduresActive.RemoveWhere (p => e.TargetIDs.Contains (p));
                    isModifiedProcedures = true;
                    break;
                case Ballot.Effect.EffectType.ReplaceProcedure:
                    _proceduresActive.Remove (e.TargetIDs[0]);
                    _proceduresActive.Add (e.TargetIDs[1]);
                    isModifiedProcedures = true;
                    break;
                case Ballot.Effect.EffectType.ModifyCurrency:
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

        VotedBallot?.Invoke (new (BallotCurrentID, isPass, Context));

        if (isModifiedProcedures) {
            OnModifiedProcedures ();
        }

        if (elections.Count > 0) {
            OnPrepareElection (elections);
        }

        if (CurrenciesValues.Count > 0) {
            OnModifiedCurrencies ();
        }
    }

    public virtual bool IsBallotPassed (IDType ballotId) => _ballotsPassed.Contains (ballotId);

    public virtual byte GetBallotsPassedCount () => (byte) _ballotsPassed.Count;

    public virtual sbyte GetCurrencyValue (IDType currencyId) => CurrenciesValues.GetValueOrDefault (currencyId);

    public virtual bool IsProcedureActive (IDType procedureId) => _proceduresActive.Contains (procedureId);
}
