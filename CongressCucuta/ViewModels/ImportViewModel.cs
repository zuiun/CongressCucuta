using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using CongressCucuta.Converters;
using CongressCucuta.Core;
using CongressCucuta.Simulations;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class ImportViewModel : ViewModel {
    internal class SimulationGroup (Func<ISimulation> generator) : ViewModel {
        public Simulation Simulation => generator ().Simulation;
        public string Name => Simulation.Localisation.State;
        public Func<ISimulation> Generator => generator;
    }

    private readonly JsonSerializerOptions _options = new () {
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
    private bool _wasImportFailure = false;
    private readonly List<SimulationGroup> _simulations;
    private int _selectedIdx = -1;
    public bool WasImportFailure {
        get => _wasImportFailure;
        set {
            _wasImportFailure = value;
            OnPropertyChanged ();
        }
    }
    public List<SimulationGroup> Simulations => _simulations;
    public int SelectedIdx {
        get => _selectedIdx;
        set {
            _selectedIdx = value;
            OnPropertyChanged ();
        }
    }
    public event Action<Simulation>? CreatingSimulation = null;

    public ImportViewModel () {
        List<Func<ISimulation>> simulations = [
            () => new Argentina (),
            () => new Australia (),
            () => new Brazil (),
            () => new Canada (),
            () => new China (),
            () => new Colombia (),
            () => new Finland (),
            () => new Hungary (),
            () => new Indonesia (),
            () => new Japan (),
            () => new Malaysia (),
            () => new Poland (),
        ];

        _simulations = [.. simulations.Select (s => new SimulationGroup (s)).OrderBy (s => s.Name)];
    }

    public void Reset () {
        WasImportFailure = false;
        SelectedIdx = -1;
    }

    public RelayCommand ChooseSimulationCommand => new (
        _ => {
            ISimulation generator = _simulations[_selectedIdx].Generator ();
            Simulation simulation = generator.Simulation;

            CreatingSimulation?.Invoke (simulation);
        },
        _ => _selectedIdx > -1
    );

    public RelayCommand ImportSimulationCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Simulation files (*.sim)|*.sim",
        };

        WasImportFailure = false;

        bool? result = file.ShowDialog ();

        if (result is not true) {
            return;
        }

        Simulation simulation;

        try {
            string json = File.ReadAllText (file.FileName);

            simulation = JsonSerializer.Deserialize<Simulation> (json, _options)!;
        } catch (Exception) {
            WasImportFailure = true;
            return;
        }

        CreatingSimulation?.Invoke (simulation);
    });
}
