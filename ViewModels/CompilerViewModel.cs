using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using congress_cucuta.Data;

namespace congress_cucuta.ViewModels;

internal class CompilerViewModel : ViewModel {
    private bool _wasCompilationSuccess = false;
    private bool _wasCompilationFailure = false;
    private readonly Simulation simulation;

    public CompilerViewModel () {
        // TODO: Must set simulation here!
        throw new NotImplementedException ();
    }

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

        if (result is null || result! == false) {
            WasCompilationSuccess = false;
            WasCompilationFailure = true;
            return;
        }

        string filename = file.FileName;
        string json = JsonSerializer.Serialize (simulation);

        try {
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
