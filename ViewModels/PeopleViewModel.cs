using System.Collections.ObjectModel;
using congress_cucuta.Data;

namespace congress_cucuta.ViewModels;

internal class PeopleViewModel : ViewModel {
    private string _name = string.Empty;
    private int _selectedIdx = -1;
    private ObservableCollection<NameViewModel> _names = [];
    private bool _wasCreationFailure = false;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public int SelectedIdx {
        get => _selectedIdx;
        set {
            _selectedIdx = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<NameViewModel> Names {
        get => _names;
        set {
            _names = value;
            OnPropertyChanged ();
        }
    }
    public bool WasCreationFailure {
        get => _wasCreationFailure;
        set {
            _wasCreationFailure = value;
            OnPropertyChanged ();
        }
    }
    public event Action<List<Person>>? InitialisingPeople = null;

    public void Reset () {
        Name = string.Empty;
        SelectedIdx = -1;
        Names = [];
        WasCreationFailure = false;
    }

    public RelayCommand AddNameCommand => new (
        _ => {
            Names.Add (new (Name));
            Name = string.Empty;
        },
        _ => !string.IsNullOrWhiteSpace (Name) && Names.Count < byte.MaxValue
    );

    public RelayCommand RemoveNameCommand => new (
        _ => Names.RemoveAt (SelectedIdx),
        _ => SelectedIdx > -1
    );

    public RelayCommand FinishInputCommand => new (
        _ => InitialisingPeople?.Invoke ([.. Names.Select ((n, i) => new Person (i, n.Name))]),
        _ => Names.Count > 1
    );
}
