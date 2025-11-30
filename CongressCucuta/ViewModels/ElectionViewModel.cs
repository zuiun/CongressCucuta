using System.Collections.ObjectModel;
using System.Diagnostics;
using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.ViewModels;

internal class ElectionViewModel : ViewModel {
    private readonly List<ElectionContext> _elections;
    private readonly HashSet<IDType> _partiesActive;
    private readonly HashSet<IDType> _regionsActive;
    private readonly Localisation _localisation;
    private readonly List<string> _people;
    private IDType _electionIdx = 0;
    private string _title = string.Empty;
    private string _target = string.Empty;
    private ObservableCollection<GroupViewModel> _groupsPeople = [];
    public Dictionary<IDType, SortedSet<IDType>> PeopleRolesNew;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactionsNew;
    public string Title {
        get => _title;
        set {
            _title = value;
            OnPropertyChanged ();
        }
    }
    public string Target {
        get => _target;
        set {
            _target = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<GroupViewModel> GroupsPeople {
        get => _groupsPeople;
        set {
            _groupsPeople = value;
            OnPropertyChanged ();
        }
    }
    public Action? CloseWindow { get; set; }
    public event Action? CompletingElection;

    public ElectionViewModel (PreparingElectionEventArgs election, ref readonly Localisation localisation) {
        _elections = election.Elections;
        _elections.Sort ();
        _partiesActive = election.PartiesActive;
        _regionsActive = election.RegionsActive;
        _people = [.. election.People.Select (kv => kv.Value.Name)];
        _localisation = localisation;
        PeopleRolesNew = election.PeopleRoles.ToDictionary (kv => kv.Key, kv => new SortedSet<IDType> ([.. kv.Value]));
        PeopleFactionsNew = election.PeopleFactions.ToDictionary (kv => kv.Key, kv => kv.Value);
    }

    private void Group_SelectedChangedEventHandler (SelectedChangedEventArgs e) {
        if (e.IsSelected) {
            if (PeopleRolesNew.TryGetValue (e.PersonID, out var rs)) {
                foreach (IDType roleId in e.TargetIDs) {
                    rs.Add (roleId);
                }
            } else {
                // Everyone should have a MEMBER Role
                throw new UnreachableException ();
            }
        } else {
            foreach (IDType roleId in e.TargetIDs) {
                PeopleRolesNew[e.PersonID].Remove (roleId);
            }
        }
    }

    public void RunElection () {
        ElectionContext election = _elections[_electionIdx];

        Title = election.Type switch {
            ElectionContext.ElectionType.ShuffleRemove =>
                $"Dissolution of {string.Join (", ", election.FilterIDs.Select (p => _localisation.GetFactionAndAbbreviation (p)))}",
            ElectionContext.ElectionType.ShuffleAdd =>
                $"Founding of {string.Join (", ", election.FilterIDs.Select (p => _localisation.GetFactionAndAbbreviation (p)))}",
            _ => _localisation.Procedures[(IDType) election.ProcedureID!].Item1,
        };
        Target = election.TargetID > 0 ? _localisation.Roles[election.TargetID].Item1 : string.Empty;
        GroupsPeople.Clear ();

        (PeopleRolesNew, PeopleFactionsNew, var groups) = election.Run (PeopleRolesNew, PeopleFactionsNew, _partiesActive, _regionsActive);

        foreach (ElectionContext.Group g in groups.Values) {
            string name;
            Dictionary<IDType, string> peopleNames = [];

            if (g.FactionID == ElectionContext.Group.NOMINEES) {
                name = "Nominees";
            } else if (g.FactionID == ElectionContext.Group.APPOINTEES) {
                name = "Appointees";
            } else {
                name = _localisation.GetFactionAndAbbreviation (g.FactionID);
            }

            foreach (IDType personId in g.PeopleAreCandidates.Keys) {
                string result = _people[personId];

                if (PeopleFactionsNew[personId].Item1 is IDType p) {
                    result += $" [{_localisation.GetFactionOrAbbreviation (p)}]";
                }

                if (PeopleFactionsNew[personId].Item2 is IDType r) {
                    result += $" [{_localisation.GetFactionOrAbbreviation (r)}]";
                }

                peopleNames[personId] = result;
            }

            GroupViewModel group = new (g, name, peopleNames);

            group.SelectedChanged += Group_SelectedChangedEventHandler;
            GroupsPeople.Add (group);
        }

        GroupsPeople = [.. GroupsPeople.OrderBy (g => g.ID)];
    }

    public RelayCommand RunNextElectionCommand => new (
        _ => {
            ++ _electionIdx;

            if (_electionIdx == _elections.Count) {
                CompletingElection?.Invoke ();
                CloseWindow?.Invoke ();
            } else {
                RunElection ();
            }
        },
        _ => GroupsPeople.All (g => g.IsSelected ())
    );

    public RelayCommand TryRunNextElectionCommand => new (_ => {
        if (RunNextElectionCommand.CanExecute (null)) {
            RunNextElectionCommand.Execute (null);
        }
    });
}
