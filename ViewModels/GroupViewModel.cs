using System.Collections.ObjectModel;
using congress_cucuta.Data;

namespace congress_cucuta.ViewModels;
internal class GroupViewModel (IDType id, string name, bool isLeaderNeeded) : ViewModel, IID {
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
                SelectedChanged?.Invoke (this, _isSelected);
            }
        }
        public event Action<PersonGroup, bool>? SelectedChanged;
    }

    private ObservableCollection<PersonGroup> _people = [];
    public IDType ID => id;
    public string Name => name;
    public bool IsLeaderNeeded => isLeaderNeeded;
    public ObservableCollection<PersonGroup> People {
        get => _people;
        set {
            _people = value;
            OnPropertyChanged ();
        }
    }

    public void AddPerson (PersonGroup person) {
        if (_people.All (p => p.ID != person.ID)) {
            _people.Add (person);
        }
    }

    public bool IsSelected () => _people.Any (p => p.IsSelected);
}
