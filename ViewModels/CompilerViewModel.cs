using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using congress_cucuta.Data;
using congress_cucuta.Simulations;
using congress_cucuta.Converters;

namespace congress_cucuta.ViewModels;

internal class CompilerViewModel : ViewModel {
    private bool _wasCompilationSuccess = false;
    private bool _wasCompilationFailure = false;
    // This is the compilation target
    private readonly Simulation _simulation = new Indonesia ().Simulation;
    private readonly JsonSerializerOptions _options = new () {
        WriteIndented = true,
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
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

    public RelayCommand CompileSimulationCommand => new (_ => {
        SaveFileDialog file = new () {
            DefaultExt = ".sim",
            Filter = "Simulation files (*.sim)| *.sim",
        };
        bool? result = file.ShowDialog ();

        if (result is null or false) {
            WasCompilationSuccess = false;
            WasCompilationFailure = true;
            return;
        }

        string filename = file.FileName;

        try {
            string json = JsonSerializer.Serialize (_simulation, _options);
            File.WriteAllText (filename, json);
        } catch (Exception) {
            WasCompilationSuccess = false;
            WasCompilationFailure = true;
            return;
        }

        WasCompilationSuccess = true;
        WasCompilationFailure = false;
    });
}
