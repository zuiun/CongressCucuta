using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using System.Collections.ObjectModel;

namespace CongressCucuta.ViewModels;

internal class SelectedChangedEventArgs (IDType personId, IDType[] targetIds, bool isSelected) {
    public IDType PersonID = personId;
    public IDType[] TargetIDs = targetIds;
    public bool IsSelected = isSelected;
}

internal class GroupViewModel : ViewModel, IID {
    internal class PersonGroup (IDType id, IDType factionId, string name, bool isCandidate = false) : ViewModel, IID {
        private bool _isSelected = false;
        public IDType ID => id;
        public IDType FactionID => factionId;
        public string Name => name;
        public bool IsCandidate => isCandidate;
        public bool IsSelected {
            get => _isSelected;
            set {
                _isSelected = value;
                OnPropertyChanged ();
                SelectedChanged?.Invoke (ID, _isSelected);
            }
        }
        public event Action<IDType, bool>? SelectedChanged;
    }

    private ObservableCollection<PersonGroup> _people = [];
    private readonly IDType[] _targetIds;
    private readonly bool _isLeaderNeeded;
    public IDType ID { get; }
    public string Name { get; }
    public ObservableCollection<PersonGroup> People {
        get => _people;
        set {
            _people = value;
            OnPropertyChanged ();
        }
    }
    public event Action<SelectedChangedEventArgs>? SelectedChanged;

    public GroupViewModel (ElectionContext.Group group, string name, Dictionary<IDType, string> peopleNames) {
        _targetIds = group.TargetIDs;
        ID = group.FactionID;
        Name = name;

        foreach (var kv in group.PeopleAreCandidates) {
            PersonGroup person = new (kv.Key, ID, peopleNames[kv.Key], kv.Value);

            person.SelectedChanged += Person_SelectedChangedEventHandler;
            People.Add (person);
        }

        People = [.. People.OrderBy (p => p.ID)];
        _isLeaderNeeded = _people.Any (p => p.IsCandidate);
    }

    private void Person_SelectedChangedEventHandler (IDType e1, bool e2) {
        SelectedChangedEventArgs args = new (e1, _targetIds, e2);

        SelectedChanged?.Invoke (args);
    }

    public bool IsSelected () => _people.Any (p => p.IsSelected) || ! _isLeaderNeeded;
}
