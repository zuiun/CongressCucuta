using congress_cucuta.Data;
using congress_cucuta.Models;
using System.Collections.ObjectModel;
using static congress_cucuta.ViewModels.PersonViewModel;

namespace congress_cucuta.ViewModels;

internal class VotingEventArgs (IDType personId, bool? isPass = null, bool? isFail = null, bool? isAbstain = null) {
    public IDType PersonID => personId;
    public bool? IsPass => isPass;
    public bool? IsFail => isFail;
    public bool? IsAbstain => isAbstain;
    public byte Votes { get; set; } = 0;
}

internal class ContextViewModel : ViewModel {
    internal class BallotGroup (string title, byte votesPass, byte votesFail, byte votesAbstain) : ViewModel {
        public string Title => title;
        public byte VotesPass => votesPass;
        public byte VotesFail => votesFail;
        public byte VotesAbstain => votesAbstain;
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
    public event Action<VotingEventArgs>? Voting;

    public ContextViewModel (ref readonly SimulationModel simulation) {
        _localisation = simulation.Localisation;
        simulation.StartingBallot += Simulation_StartingBallotEventHandler;
        simulation.EndingBallot += Simulation_EndingBallotEventHandler;
        ;
        simulation.Context.InitialisedPeople += Context_InitialisedPeopleEventHandler;
        simulation.Context.CompletedElection += Context_CompletedElectionEventHandler;
        simulation.Context.UpdatedPermissions += Context_UpdatedPermissionsEventHandler;
        simulation.Context.VotedBallot += Context_VotedBallotEventHandler;
        simulation.Context.ModifiedCurrencies += Context_ModifiedCurrenciesEventHandler;
    }

    private PersonViewModel CreatePerson (IDType id, string name, SortedSet<IDType> roles) {
        PersonViewModel person = new (id, name);

        person.Roles.Clear ();

        foreach (IDType r in roles) {
            if (r != Role.MEMBER) {
                person.Roles.Add (new (_localisation.Roles[r].Item1));
            }
        }

        person.VotingPass += Person_VotingPassEventHandler;
        person.VotingFail += Person_VotingFailEventHandler;
        person.VotingAbstain += Person_VotingAbstainEventHandler;
        return person;
    }

    private void CreateFactionsPeople (
        Dictionary<IDType, Person> people,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions
    ) {
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
                    party = new FactionViewModel (pa, _localisation.GetFactionAndAbbreviation (pa));
                    _factionsPeople.Add (party);
                }

                PersonViewModel person = CreatePerson (kv.Key, people[kv.Key].Name, peopleRoles[kv.Key]);

                party.People.Add (person);
            }

            if (regionId is IDType r) {
                FactionViewModel region;

                if (_factionsPeople.Any (f => f.ID == r)) {
                    region = _factionsPeople.Where (f => f.ID == r).First ();
                } else {
                    region = new FactionViewModel (r, _localisation.GetFactionAndAbbreviation (r));
                    _factionsPeople.Add (region);
                }

                PersonViewModel person = CreatePerson (kv.Key, people[kv.Key].Name, peopleRoles[kv.Key]);

                region.People.Add (person);
            }
        }
        
        foreach (FactionViewModel f in _factionsPeople) {
            f.Sort ();
        }

        FactionsPeople = [.. _factionsPeople.OrderBy (f => f.ID)];
    }

    private void CreatePeople (Dictionary<IDType, Person> people, Dictionary<IDType, SortedSet<IDType>> peopleRoles) {
        _people.Clear ();
        _factionsPeople.Clear ();

        foreach (Person p in people.Values) {
            PersonViewModel person = CreatePerson (p.ID, p.Name, peopleRoles[p.ID]);

            _people.Add (person);
        }

        People = [.. _people.OrderBy (p => p.ID)];
    }

    private void Simulation_StartingBallotEventHandler (StartingBallotEventArgs e) {
        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.IsInteractable = true;
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                f.SetInteractability (true);
            }
        }

        VotesPassThreshold = e.VotesPassThreshold;
        VotesFailThreshold = e.VotesFailThreshold;
        VotesPass = e.VotesPass;
        VotesFail = e.VotesFail;
        VotesAbstain = e.VotesTotal;
        IsBallotCount = true;
    }

    private void Simulation_EndingBallotEventHandler () {
        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.IsInteractable = false;
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                f.SetInteractability (false);
            }
        }

        VotesPassThreshold = 0;
        VotesFailThreshold = 0;
        VotesPass = 0;
        VotesFail = 0;
        VotesAbstain = 0;
        IsBallotCount = false;
    }

    private void Context_InitialisedPeopleEventHandler (Dictionary<IDType, Person> e) {
        foreach (Person p in e.Values) {
            PersonViewModel person = CreatePerson (p.ID, p.Name, []);

            _people.Add (person);
        }
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
        VotesPass = args.Votes;
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
        VotesFail = args.Votes;
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
        VotesAbstain = args.Votes;
    }

    private void Context_CompletedElectionEventHandler (CompletedElectionEventArgs e) {
        IsFaction = _localisation.Regions.Count > 0 || _localisation.Parties.Count > 0;
        IsPeople = ! IsFaction;

        if (IsFaction) {
            CreateFactionsPeople (e.People, e.PeopleRoles, e.PeopleFactions);
        } else {
            CreatePeople (e.People, e.PeopleRoles);
        }
    }

    private void Context_UpdatedPermissionsEventHandler (Dictionary<IDType, Permissions> e) {
        if (IsPeople) {
            foreach (PersonViewModel p in _people) {
                p.UpdatePermissions (e[p.ID]);
            }
        } else {
            foreach (FactionViewModel f in _factionsPeople) {
                f.UpdatePermissions (e);
            }
        }
    }

    private void Context_VotedBallotEventHandler (VotedBallotEventArgs e) => Ballots.Add (new (
            _localisation.Ballots[e.ID].Item1,
            e.VotesPass,
            e.VotesFail,
            e.VotesAbstain
        ));

    private void Context_ModifiedCurrenciesEventHandler (Dictionary<IDType, sbyte> e) {
        foreach (var kv in e) {
            if (kv.Key == Currency.STATE) {
                IsCurrency = true;
                Value = kv.Value;
                CurrencyName = _localisation.Currencies[kv.Key];
            } else if (_factionsPeople.Any (f => f.ID == kv.Key)) {
                FactionViewModel faction = _factionsPeople.Where (f => f.ID == kv.Key).First ();

                faction.IsCurrency = true;
                faction.IsNotCurrency = false;
                faction.Value = kv.Value;
                faction.Currency = _localisation.Currencies[kv.Key];
            }
        }
    }
}
