using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using CongressCucuta.Data;

namespace CongressCucuta.ViewModels;

internal class PeopleViewModel : ViewModel {
    private const byte PEOPLE_MIN = 5;
    private string _name = string.Empty;
    private int _selectedIdx = -1;
    private ObservableCollection<NameViewModel> _names = [];
    private bool _wasCreationFailure = false;
    private bool _wasImportFailure = false;
    private bool _wasImportSuccess = false;
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
    public bool WasImportFailure {
        get => _wasImportFailure;
        set {
            _wasImportFailure = value;
            OnPropertyChanged ();
        }
    }
    public bool WasImportSuccess {
        get => _wasImportSuccess;
        set {
            _wasImportSuccess = value;
            OnPropertyChanged ();
        }
    }
    public static byte PeopleMin => PEOPLE_MIN;
    public event Action<List<Person>>? InitialisingPeople = null;

    public void Reset () {
        Name = string.Empty;
        SelectedIdx = -1;
        Names = [];
        WasCreationFailure = false;
        WasImportFailure = false;
        WasImportSuccess = false;
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

    public RelayCommand ImportNamesCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Text files (*.txt)|*.txt",
        };

        WasImportFailure = false;
        WasImportSuccess = false;

        bool? result = file.ShowDialog ();

        if (result != true) {
            WasImportFailure = true;
            return;
        }

        try {
            string[] names = File.ReadAllLines (file.FileName);

            Names = [.. names.Select (n => new NameViewModel (n))];
        } catch (Exception) {
            WasImportFailure = true;
            return;
        }

        WasImportSuccess = true;
    });

    public RelayCommand FinishInputCommand => new (
        _ => InitialisingPeople?.Invoke ([.. Names.Select ((n, i) => new Person (i, n.Name))]),
        _ => Names.Count >= PEOPLE_MIN
    );
}
