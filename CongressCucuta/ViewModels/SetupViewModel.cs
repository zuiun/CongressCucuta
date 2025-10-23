using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;
using CongressCucuta.Views;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class SetupViewModel : ViewModel {
    private const string CONGRESS_CUCUTA = "Congress of Cúcuta";
    private Task<SimulationViewModel>? _simulation = null;
    private bool _isImportSetup = true;
    private bool _isPeopleSetup = false;
    private readonly ImportViewModel _import = new ();
    private readonly PeopleViewModel _people = new ();
    private string _title = CONGRESS_CUCUTA;
    public bool IsImportSetup {
        get => _isImportSetup;
        set {
            _isImportSetup = value;
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
    public ImportViewModel Import => _import;
    public PeopleViewModel People => _people;
    public string Title {
        get => _title;
        set {
            _title = value;
            OnPropertyChanged ();
        }
    }

    public SetupViewModel () {
        _import.CreatingSimulation += Import_CreatingSimulationEventHandler;
        _people.InitialisingPeople += People_InitialisingPeopleEventHandler;
        _people.CreatingSimulation += People_CreatingSimulationEventHandler;
    }

    private void Reset () {
        Title = CONGRESS_CUCUTA;
        _import.Reset ();
        _people.Reset ();
        IsImportSetup = true;
        IsPeopleSetup = false;
        _simulation = null;
    }

    private void Import_CreatingSimulationEventHandler (Simulation simulation) {
        Title = simulation.Localisation.State;
        _import.Reset ();
        _people.Reset ();
        IsImportSetup = false;
        IsPeopleSetup = true;
        _simulation = Task.Run (() => new SimulationViewModel (simulation));
    }

    private void People_CreatingSimulationEventHandler () => Reset ();

    private async void People_InitialisingPeopleEventHandler (List<Person> people) {
        SimulationViewModel simulation;

        try {
            simulation = await _simulation!;
        } catch (Exception) {
            _people.WasCreationFailure = true;
            return;
        }

        simulation.InitialisePeople (people);

        SimulationWindow window = new () {
            DataContext = simulation,
        };

        Reset ();
        window.ShowDialog ();
    }
}
