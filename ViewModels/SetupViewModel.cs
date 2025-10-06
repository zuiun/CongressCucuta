using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using congress_cucuta.Converters;
using congress_cucuta.Data;
using congress_cucuta.Views;

namespace congress_cucuta.ViewModels;

internal class SetupViewModel : ViewModel {
    private readonly JsonSerializerOptions _options = new () {
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
    private Task<SimulationViewModel>? _simulation = null;
    private bool _isFileSetup = true;
    private bool _isPeopleSetup = false;
    private bool _wasChoiceFailure = false;
    private string _name = string.Empty;
    private int _selectedIdx = -1;
    private readonly ObservableCollection<NameViewModel> _names = [];
    private bool _wasCreationFailure = false;
    public bool IsFileSetup {
        get => _isFileSetup;
        set {
            _isFileSetup = value;
            OnPropertyChanged ();
        }
    }
    public bool IsPeopleSetup {
        get => _isPeopleSetup;
        set {
            _isPeopleSetup = value;
            OnPropertyChanged ();
        }
    }
    public bool WasChoiceFailure {
        get => _wasChoiceFailure;
        set {
            _wasChoiceFailure = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<NameViewModel> Names => _names;
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
    public bool WasCreationFailure {
        get => _wasCreationFailure;
        set {
            _wasCreationFailure = value;
            OnPropertyChanged ();
        }
    }

    public RelayCommand ChooseSimulationCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Simulation files (*.sim)|*.sim",
        };
        bool? result = file.ShowDialog ();

        // TODO: Enable when context is done
        if (result! == true) {
            WasChoiceFailure = false;
        } else {
            WasChoiceFailure = true;
            return;
        }

        Simulation simulation;

        try {
            string json = File.ReadAllText (file.FileName);
            
            simulation = JsonSerializer.Deserialize<Simulation> (json, _options)!;
        } catch (Exception) {
            WasChoiceFailure = true;
            return;
        }

        IsFileSetup = false;
        IsPeopleSetup = true;
        WasCreationFailure = false;
        _simulation = Task.Run (() => new SimulationViewModel (simulation));
    });

    public RelayCommand AddNameCommand => new (
        _ => {
            Names.Add (new (Name));
            Name = string.Empty;
        },
        _ => ! string.IsNullOrWhiteSpace (Name)
    );

    public RelayCommand RemoveNameCommand => new (
        _ => Names.RemoveAt (SelectedIdx),
        _ => SelectedIdx > -1
    );

    public RelayCommand FinishInputCommand => new (
        async _ => {
            List<Person> people = [.. Names.Select ((n, i) => new Person (i, n.Name))];
            SimulationViewModel simulation;

            try {
                simulation = await _simulation!;
            } catch (Exception) {
                WasCreationFailure = true;
                return;
            }

            simulation.Simulation.Context.InitialisePeople (people);

            SimulationWindow window = new () {
                DataContext = simulation,
            };

            IsPeopleSetup = false;
            IsFileSetup = true;
            Name = string.Empty;
            Names.Clear ();
            SelectedIdx = -1;
            window.ShowDialog ();
        },
        _ => Names.Count > 0
    );
}
