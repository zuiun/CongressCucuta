using CongressCucuta.Data;
using CongressCucuta.Views;

namespace CongressCucuta.ViewModels;

internal class SetupViewModel : ViewModel {
    private Task<SimulationViewModel>? _simulation = null;
    private bool _isFileSetup = true;
    private bool _isPeopleSetup = false;
    private readonly FileViewModel _file = new ();
    private readonly PeopleViewModel _people = new ();
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
    public FileViewModel File => _file;
    public PeopleViewModel People => _people;

    public SetupViewModel () {
        _file.CreatingSimulation += File_CreatingSimulationEventHandler;
        _people.InitialisingPeople += People_InitialisingPeopleEventHandler;
    }

    private void File_CreatingSimulationEventHandler (Simulation simulation) {
        IsFileSetup = false;
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

        _file.Reset ();
        _people.Reset ();
        IsPeopleSetup = false;
        IsFileSetup = true;
        window.ShowDialog ();
    }
}
