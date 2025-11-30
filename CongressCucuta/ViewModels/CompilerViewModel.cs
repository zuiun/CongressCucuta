using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using CongressCucuta.Converters;
using CongressCucuta.Core;
using CongressCucuta.Simulations;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class CompilerViewModel : ViewModel {
    internal class SimulationGroup (ISimulation simulation) : ViewModel {
        public Simulation Simulation => simulation.Simulation;
        public string Name => simulation.Simulation.Localisation.State;
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
    private readonly List<SimulationGroup> _simulations;
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
        _simulations = [
            new (new Argentina ()),
            new (new Australia ()),
            new (new Brazil ()),
            new (new Canada ()),
            new (new China ()),
            new (new Colombia ()),
            new (new Finland ()),
            new (new Hungary ()),
            new (new Indonesia ()),
            new (new Japan ()),
            new (new Malaysia ()),
            new (new Poland ()),
        ];
        _simulations = [.. _simulations.OrderBy (s => s.Name)];
    }

    public RelayCommand CompileSimulationCommand => new (
        _ => {
            SaveFileDialog file = new () {
                DefaultExt = ".sim",
                Filter = "Simulation files (*.sim)| *.sim",
                FileName = _simulations[_selectedIdx].Name.ToLower (),
            };

            WasCompilationSuccess = false;
            WasCompilationFailure = false;
            Name = _simulations[_selectedIdx].Name;

            bool? result = file.ShowDialog ();

            if (result is not true) {
                return;
            }

            string filename = file.FileName;

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
        },
        _ => _selectedIdx > -1
    );
}
