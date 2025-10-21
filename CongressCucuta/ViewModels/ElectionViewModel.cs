using CongressCucuta.Core;
using CongressCucuta.Models;

namespace CongressCucuta.ViewModels;

internal class ElectionViewModel : ViewModel {
    private readonly List<ElectionModel> _elections;
    private readonly HashSet<IDType> _partiesActive;
    private readonly HashSet<IDType> _regionsActive;
    private readonly Localisation _localisation;
    private readonly List<string> _people;
    private IDType _electionIdx = 0;
    private ElectionTypeViewModel _election;
    public readonly Dictionary<IDType, SortedSet<IDType>> PeopleRolesNew;
    public readonly Dictionary<IDType, (IDType?, IDType?)> PeopleFactionsNew;
    public Action? CloseWindow { get; set; }
    public event Action? CompletedElection;

    public ElectionTypeViewModel Election {
        get => _election;
        set {
            _election = value;
            OnPropertyChanged ();
        }
    }

    public ElectionViewModel (CompletingElectionEventArgs election) {
        _elections = election.Elections;
        _partiesActive = election.PartiesActive;
        _regionsActive = election.RegionsActive;
        _people = [.. election.PeopleNames.Select (kv => kv.Value)];
        _localisation = election.Localisation;
        PeopleRolesNew = election.PeopleRoles.ToDictionary (kv => kv.Key, kv => new SortedSet<IDType> ([.. kv.Value]));
        PeopleFactionsNew = election.PeopleFactions.ToDictionary (kv => kv.Key, kv => kv.Value);
        _election = ElectionTypeViewModel.Create (
            _elections[_electionIdx],
            PeopleRolesNew,
            PeopleFactionsNew,
            _partiesActive,
            _regionsActive,
            _localisation,
            _people
        );
    }

    public RelayCommand RunNextElectionCommand => new (
        _ => {
            foreach (var kv in _election.PeopleRolesNew) {
                PeopleRolesNew[kv.Key] = kv.Value;
            }

            foreach (var kv in _election.PeopleFactionsNew) {
                PeopleFactionsNew[kv.Key] = kv.Value;
            }

            ++ _electionIdx;

            if (_electionIdx == _elections.Count) {
                CompletedElection?.Invoke ();
                CloseWindow?.Invoke ();
            } else {
                Election = ElectionTypeViewModel.Create (
                    _elections[_electionIdx],
                    PeopleRolesNew,
                    PeopleFactionsNew,
                    _partiesActive,
                    _regionsActive,
                    _localisation,
                    _people
                );
            }
        },
        _ => _electionIdx < _elections.Count && _election.CanContinue ()
    );
}
