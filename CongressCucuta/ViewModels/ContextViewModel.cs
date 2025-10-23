using System.Collections.ObjectModel;
using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.ViewModels;

internal class VotingEventArgs (IDType personId, bool? isPass = null, bool? isFail = null, bool? isAbstain = null) {
    public IDType PersonID => personId;
    public bool? IsPass => isPass;
    public bool? IsFail => isFail;
    public bool? IsAbstain => isAbstain;
    public byte VotesPass { get; set; } = 0;
    public byte VotesFail { get; set; } = 0;
    public byte VotesAbstain { get; set; } = 0;
}

internal class ContextViewModel : ViewModel {
    internal class BallotGroup (
        string title,
        string name,
        byte votesPass,
        byte votesFail,
        byte votesAbstain,
        bool isPass,
        List<string> procedures
    ) : ViewModel {
        public string Title => title;
        public string Name => name;
        public byte VotesPass => votesPass;
        public byte VotesFail => votesFail;
        public byte VotesAbstain => votesAbstain;
        public bool IsPass => isPass;
        public ObservableCollection<string> Procedures => [.. procedures];
    }

    internal class ProcedureGroup : ViewModel, IID {
        public IDType ID { get; }
        public string Name { get; }
        public string Effects { get; }

        public ProcedureGroup (IDType id, string name, string effects) {
            string[] lines = effects.Split ('\n');
            string[] clean = lines[1 ..];
            string join = string.Join ('\n', clean);
            string trim = StringLineFormatter.Outdent (join);

            ID = id;
            Name = name;
            Effects = StringLineFormatter.Convert (trim);
        }
    }

    private readonly Localisation _localisation;
    private bool _isPeople = true;
    private bool _isFaction = false;
    // One of the two is used, depending on if another faction is enabled
    private ObservableCollection<PersonViewModel> _people = [];
    private ObservableCollection<FactionViewModel> _factionsPeople = [];
    private readonly ObservableCollection<BallotGroup> _ballots = [];
    private byte _votesPass = 0;
    private byte _votesFail = 0;
    private byte _votesAbstain = 0;
    private sbyte _value = 0;
    private string _currency = string.Empty;
    private bool _isCurrency = false;
    private bool _isBallotCount = false;
    private byte _votesPassThreshold = 0;
    private byte _votesFailThreshold = 0;
    private ObservableCollection<ProcedureGroup> _proceduresGovernmental = [];
    private ObservableCollection<ProcedureGroup> _proceduresSpecial = [];
    private ObservableCollection<ProcedureGroup> _proceduresDeclared = [];
    private readonly HashSet<IDType> _declarerRoles = [];
    private readonly Dictionary<IDType, string> _proceduresEffects = [];
    public bool IsPeople {
        get => _isPeople;
        set {
            _isPeople = value;
            OnPropertyChanged ();
        }
    }
    public bool IsFaction {
        get => _isFaction;
        set {
            _isFaction = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<PersonViewModel> People {
        get => _people;
        set {
            _people = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<FactionViewModel> FactionsPeople {
        get => _factionsPeople;
        set {
            _factionsPeople = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<BallotGroup> Ballots => _ballots;
    public byte VotesPass {
        get => _votesPass;
        set {
            _votesPass = value;
            OnPropertyChanged ();
        }
    }
    public byte VotesFail {
        get => _votesFail;
        set {
            _votesFail = value;
            OnPropertyChanged ();
        }
    }
    public byte VotesAbstain {
        get => _votesAbstain;
        set {
            _votesAbstain = value;
            OnPropertyChanged ();
        }
    }
    public sbyte Value {
        get => _value;
        set {
            _value = value;
            OnPropertyChanged ();
        }
    }
    public string CurrencyName {
        get => _currency;
        set {
            _currency = value;
            OnPropertyChanged ();
        }
    }
    public bool IsCurrency {
        get => _isCurrency;
        set {
            _isCurrency = value;
            OnPropertyChanged ();
        }
    }
    public bool IsBallotCount {
        get => _isBallotCount;
        set {
            _isBallotCount = value;
            OnPropertyChanged ();
        }
    }
    public byte VotesPassThreshold {
        get => _votesPassThreshold;
        set {
            _votesPassThreshold = value;
            OnPropertyChanged ();
        }
    }
    public byte VotesFailThreshold {
        get => _votesFailThreshold;
        set {
            _votesFailThreshold = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<ProcedureGroup> ProceduresGovernmental {
        get => _proceduresGovernmental;
        set {
            _proceduresGovernmental = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<ProcedureGroup> ProceduresSpecial {
        get => _proceduresSpecial;
        set {
            _proceduresSpecial = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<ProcedureGroup> ProceduresDeclared {
        get => _proceduresDeclared;
        set {
            _proceduresDeclared = value;
            OnPropertyChanged ();
        }
    }
    public event Action<VotingEventArgs>? Voting;
    public event Action<IDType>? DeclaringProcedure;

    public ContextViewModel (SimulationContext context, Simulation simulation, ref readonly Localisation localisation) {
        _localisation = localisation;
        context.CompletedElection += Context_CompletedElectionEventHandler;
        context.UpdatedPermissions += Context_UpdatedPermissionsEventHandler;
        context.VotedBallot += Context_VotedBallotEventHandler;
        context.ModifiedProcedures += Simulation_ModifiedProceduresEventHandler;
        context.ModifiedCurrencies += Context_ModifiedCurrenciesEventHandler;
        context.Context.ResetVotes += Context_ResetVotesEventHandler;

        foreach (ProcedureDeclared pd in context.ProceduresDeclared.Values) {
            if (pd.DeclarerIDs.Length > 0) {
                foreach (IDType r in pd.DeclarerIDs) {
                    _declarerRoles.Add (r);
                }
            } else {
                _declarerRoles.Add (Role.MEMBER);
            }
        }

        foreach (ProcedureImmediate pi in context.ProceduresGovernmental.Values) {
            string effect = pi.ToString (simulation, in _localisation);
            ProcedureGroup procedure = new (
                pi.ID,
                _localisation.Procedures[pi.ID].Item1,
                effect
            );

            ProceduresGovernmental.Add (procedure);
            _proceduresEffects[pi.ID] = effect;
        }

        foreach (ProcedureTargeted pt in context.ProceduresSpecial.Values) {
            string effect = pt.ToString (simulation, in _localisation);

            if (pt.IsActiveStart) {
                ProcedureGroup procedure = new (
                    pt.ID,
                    _localisation.Procedures[pt.ID].Item1,
                    effect
                );

                ProceduresSpecial.Add (procedure);
            }

            _proceduresEffects[pt.ID] = effect;
        }

        foreach (ProcedureDeclared pd in context.ProceduresDeclared.Values) {
            string effect = pd.ToString (simulation, in _localisation);
            ProcedureGroup procedure = new (
                pd.ID,
                _localisation.Procedures[pd.ID].Item1,
                effect
            );

            ProceduresDeclared.Add (procedure);
            _proceduresEffects[pd.ID] = effect;
        }

        Sort ();
    }

    public void InitialisePeople (List<Person> people) {
        foreach (Person p in people) {
            PersonViewModel person = CreatePerson (p.ID, p.Name, [], false);

            _people.Add (person);
        }
    }

    private void Sort () {
        ProceduresGovernmental = [.. ProceduresGovernmental.OrderBy (p => p.ID)];
        ProceduresSpecial = [.. ProceduresSpecial.OrderBy (p => p.ID)];
        ProceduresDeclared = [.. ProceduresDeclared.OrderBy (p => p.ID)];
    }

    private void SetCurrencies (Dictionary<IDType, sbyte> currenciesValues) {
        foreach (var kv in currenciesValues) {
            if (kv.Key == Currency.STATE) {
                IsCurrency = true;
                Value = kv.Value;
                CurrencyName = _localisation.Currencies[kv.Key];
            } else {
                var factionIter = _factionsPeople.Where (f => f.ID == kv.Key);

                if (factionIter.Any ()) {
                    FactionViewModel faction = factionIter.First ();

                    faction.IsCurrency = true;
                    faction.IsNotCurrency = false;
                    faction.Value = kv.Value;
                    faction.Currency = _localisation.Currencies[kv.Key];
                }
            }
        }
    }

    private PersonViewModel CreatePerson (IDType id, string name, SortedSet<IDType> roles, bool isBallot) {
        PersonViewModel person = new (id, name, isBallot);

        foreach (IDType r in roles) {
            if (r != Role.MEMBER && r != Role.LEADER_PARTY && r != Role.LEADER_REGION) {
                person.Roles.Add (new (r, _localisation.Roles[r].Item1));
            }

            if (_declarerRoles.Contains (r)) {
                person.CanDeclare = true;
            }
        }

        person.VotingPass += Person_VotingPassEventHandler;
        person.VotingFail += Person_VotingFailEventHandler;
        person.VotingAbstain += Person_VotingAbstainEventHandler;
        person.DeclaringProcedure += Person_DeclaringProcedureEventHandler;
        return person;
    }

    private void CreateFactionsPeople (
        Dictionary<IDType, Person> people,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        bool isBallot
    ) {
        Dictionary<IDType, sbyte> currenciesValues = [];

        foreach (FactionViewModel f in _factionsPeople) {
            if (f.IsCurrency) {
                currenciesValues[f.ID] = f.Value;
            }
        }

        _people.Clear ();
        _factionsPeople.Clear ();
        
        foreach (var kv in peopleFactions) {
            IDType? partyId = kv.Value.Item1;
            IDType? regionId = kv.Value.Item2;

            if (partyId is IDType pa) {
                FactionViewModel party;

                if (_factionsPeople.Any (f => f.ID == pa)) {
                    party = _factionsPeople.Where (f => f.ID == pa).First ();
                } else {
                    party = new FactionViewModel (pa, _localisation.GetFactionOrAbbreviation (pa));
                    _factionsPeople.Add (party);

                    if (_localisation.Abbreviations.ContainsKey (pa)) {
                        party.Description = _localisation.Parties[pa].Item1;
                    }
                }

                PersonViewModel person = CreatePerson (kv.Key, people[kv.Key].Name, peopleRoles[kv.Key], isBallot);

                party.People.Add (person);
            }

            if (regionId is IDType r) {
                FactionViewModel region;

                if (_factionsPeople.Any (f => f.ID == r)) {
                    region = _factionsPeople.Where (f => f.ID == r).First ();
                } else {
                    region = new FactionViewModel (r, _localisation.GetFactionOrAbbreviation (r));
                    _factionsPeople.Add (region);

                    if (_localisation.Abbreviations.ContainsKey (r)) {
                        region.Description = _localisation.Regions[r].Item1;
                    }
                }

                PersonViewModel person = CreatePerson (kv.Key, people[kv.Key].Name, peopleRoles[kv.Key], isBallot);

                region.People.Add (person);
            }

            if (partyId is null && regionId is null) {
                FactionViewModel independent;

                if (_factionsPeople.Any (f => f.ID == FactionViewModel.INDEPENDENT)) {
                    independent = _factionsPeople.Where (f => f.ID == FactionViewModel.INDEPENDENT).First ();
                } else {
                    independent = new (FactionViewModel.INDEPENDENT, "Independent");
                    _factionsPeople.Add (independent);
                }

                PersonViewModel person = CreatePerson (kv.Key, people[kv.Key].Name, peopleRoles[kv.Key], isBallot);

                independent.People.Add (person);
            }
        }
        
        foreach (FactionViewModel f in _factionsPeople) {
            f.Sort ();
        }

        FactionsPeople = [.. _factionsPeople.OrderBy (f => f.ID)];
        SetCurrencies (currenciesValues);
    }

    private void CreatePeople (Dictionary<IDType, Person> people, Dictionary<IDType, SortedSet<IDType>> peopleRoles, bool isBallot) {
        _people.Clear ();
        _factionsPeople.Clear ();

        foreach (Person p in people.Values) {
            PersonViewModel person = CreatePerson (p.ID, p.Name, peopleRoles[p.ID], isBallot);

            _people.Add (person);
        }

        People = [.. _people.OrderBy (p => p.ID)];
    }

    public void Simulation_StartingBallotEventHandler (StartingBallotEventArgs e) {
        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.IsInteractable = true;
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                foreach (PersonViewModel p in f.People) {
                    p.IsInteractable = true;
                }
            }
        }

        VotesPassThreshold = e.VotesPassThreshold;
        VotesFailThreshold = e.VotesFailThreshold;
        VotesPass = e.VotesPass;
        VotesFail = e.VotesFail;
        VotesAbstain = e.VotesAbstain;
        IsBallotCount = true;
    }

    public void Simulation_EndingBallotEventHandler () {
        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.IsInteractable = false;
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                foreach (PersonViewModel p in f.People) {
                    p.IsInteractable = false;
                }
            }
        }

        VotesPassThreshold = 0;
        VotesFailThreshold = 0;
        VotesPass = 0;
        VotesFail = 0;
        VotesAbstain = 0;
        IsBallotCount = false;
    }

    private void Person_VotingPassEventHandler (object? sender, bool e) {
        IDType id = ((PersonViewModel) sender!).ID;
        VotingEventArgs args = new (
            id,
            e,
            null,
            null
        );

        Voting?.Invoke (args);
        VotesPass = args.VotesPass;
        VotesFail = args.VotesFail;
        VotesAbstain = args.VotesAbstain;
    }

    private void Person_VotingFailEventHandler (object? sender, bool e) {
        IDType id = ((PersonViewModel) sender!).ID;
        VotingEventArgs args = new (
            id,
            null,
            e,
            null
        );

        Voting?.Invoke (args);
        VotesPass = args.VotesPass;
        VotesFail = args.VotesFail;
        VotesAbstain = args.VotesAbstain;
    }

    private void Person_VotingAbstainEventHandler (object? sender, bool e) {
        IDType id = ((PersonViewModel) sender!).ID;
        VotingEventArgs args = new (
            id,
            null,
            null,
            e
        );

        Voting?.Invoke (args);
        VotesPass = args.VotesPass;
        VotesFail = args.VotesFail;
        VotesAbstain = args.VotesAbstain;
    }

    private void Context_ResetVotesEventHandler (ResetVotesEventArgs e) {
        VotesPass = e.VotesPass;
        VotesFail = e.VotesFail;
        VotesAbstain = e.VotesAbstain;
        VotesPassThreshold = e.VotesPassThreshold;
        VotesFailThreshold = e.VotesFailThreshold;

        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.Reset ();
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                foreach (PersonViewModel p in f.People) {
                    p.Reset ();
                }
            }
        }
    }

    private void Person_DeclaringProcedureEventHandler (IDType e) {
        DeclaringProcedure?.Invoke (e);
    }

    private void Context_CompletedElectionEventHandler (CompletedElectionEventArgs e) {
        IsFaction = _localisation.Regions.Count > 0 || _localisation.Parties.Count > 0;
        IsPeople = ! IsFaction;

        if (IsFaction) {
            CreateFactionsPeople (e.People, e.PeopleRoles, e.PeopleFactions, e.IsBallot);
        } else {
            CreatePeople (e.People, e.PeopleRoles, e.IsBallot);
        }
    }

    private void Context_UpdatedPermissionsEventHandler (Dictionary<IDType, Permissions> e) {
        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.UpdatePermissions (e[p.ID]);
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                foreach (PersonViewModel p in f.People) {
                    p.UpdatePermissions (e[p.ID]);
                }
            }
        }
    }

    private void Context_VotedBallotEventHandler (VotedBallotEventArgs e) => Ballots.Add (new (
        _localisation.Ballots[e.ID].Item1,
        _localisation.Ballots[e.ID].Item2,
        e.VotesPass,
        e.VotesFail,
        e.VotesAbstain,
        e.IsPassed,
        e.ProceduresDeclared.ConvertAll (p => _localisation.Procedures[p.ID].Item1)
    ));

    private void Simulation_ModifiedProceduresEventHandler (HashSet<ProcedureTargeted> e) {
        ProceduresSpecial.Clear ();

        foreach (ProcedureTargeted pt in e) {
            ProcedureGroup procedure = new (
                pt.ID,
                _localisation.Procedures[pt.ID].Item1,
                _proceduresEffects[pt.ID]
            );

            ProceduresSpecial.Add (procedure);
        }

        Sort ();
    }

    private void Context_ModifiedCurrenciesEventHandler (Dictionary<IDType, sbyte> e) => SetCurrencies (e);
}
