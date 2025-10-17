using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using CongressCucuta.Data;
using CongressCucuta.Simulations;
using CongressCucuta.Converters;

namespace CongressCucuta.ViewModels;

internal class CompilerViewModel : ViewModel {
    internal class SimulationGroup (ISimulation simulation) : ViewModel {
        public Simulation Simulation => simulation.Simulation;
        public string Name => Simulation.Localisation.State;
    }

    private bool _wasCompilationSuccess = false;
    private bool _wasCompilationFailure = false;
    private readonly JsonSerializerOptions _options = new () {
        WriteIndented = true,
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
    private string _name = string.Empty;
    private string _filename = string.Empty;
    private readonly List<SimulationGroup> _simulations = [];
    private int _selectedIdx = -1;
    public bool WasCompilationSuccess {
        get => _wasCompilationSuccess;
        set {
            _wasCompilationSuccess = value;
            OnPropertyChanged ();
        }
    }
    public bool WasCompilationFailure {
        get => _wasCompilationFailure;
        set {
            _wasCompilationFailure = value;
            OnPropertyChanged ();
        }
    }
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public string Filename {
        get => _filename;
        set {
            _filename = value;
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

    public CompilerViewModel () {
        // Compilation targets
        _simulations.Add (new (new Argentina ()));
        _simulations.Add (new (new Australia ()));
        _simulations.Add (new (new Brazil ()));
        _simulations.Add (new (new Canada ()));
        _simulations.Add (new (new China ()));
        _simulations.Add (new (new Colombia ()));
        _simulations.Add (new (new Finland ()));
        _simulations.Add (new (new Hungary ()));
        _simulations.Add (new (new Indonesia ()));
        _simulations.Add (new (new Japan ()));
        _simulations.Add (new (new Malaysia ()));
        _simulations.Add (new (new Poland ()));
        _simulations = [.. _simulations.OrderBy (s => s.Name)];
    }

    public RelayCommand CompileSimulationCommand => new (_ => {
        if (_selectedIdx > -1) {
            SaveFileDialog file = new () {
                DefaultExt = ".sim",
                Filter = "Simulation files (*.sim)| *.sim",
                FileName = _simulations[_selectedIdx].Name.ToLower (),
            };

            WasCompilationSuccess = false;
            WasCompilationFailure = false;
            Name = _simulations[_selectedIdx].Name;
            Filename = "a .sim file";

            bool? result = file.ShowDialog ();

            if (result is null or false) {
                WasCompilationSuccess = false;
                WasCompilationFailure = true;
                return;
            }

            string filename = file.FileName;

            Name = _simulations[_selectedIdx].Name;
            Filename = Path.GetFileName (filename);

            try {
                string json = JsonSerializer.Serialize (_simulations[_selectedIdx].Simulation, _options);
                File.WriteAllText (filename, json);
            } catch (Exception) {
                WasCompilationSuccess = false;
                WasCompilationFailure = true;
                return;
            }

            WasCompilationSuccess = true;
            WasCompilationFailure = false;
        }
    });
}
