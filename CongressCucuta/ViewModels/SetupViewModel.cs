using CongressCucuta.Core;
using CongressCucuta.Views;

namespace CongressCucuta.ViewModels;

internal class SetupViewModel : ViewModel {
    private Task<SimulationViewModel>? _simulation = null;
    private bool _isImportSetup = true;
    private bool _isPeopleSetup = false;
    private readonly ImportViewModel _import = new ();
    private readonly PeopleViewModel _people = new ();
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

    public SetupViewModel () {
        _import.CreatingSimulation += Import_CreatingSimulationEventHandler;
        _people.InitialisingPeople += People_InitialisingPeopleEventHandler;
    }

    private void Import_CreatingSimulationEventHandler (Simulation simulation) {
        IsImportSetup = false;
        IsPeopleSetup = true;
        _simulation = Task.Run (() => new SimulationViewModel (simulation));
    }

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

        _import.Reset ();
        _people.Reset ();
        IsPeopleSetup = false;
        IsImportSetup = true;
        window.ShowDialog ();
    }
}
