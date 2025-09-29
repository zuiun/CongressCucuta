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
        // Must set simulation here!
        this.simulation = new ();
    }

    public Boolean WasCompilationSuccess {
        get => this._wasCompilationSuccess;
        set {
            this._wasCompilationSuccess = value;
            OnPropertyChanged ();
        }
    }

    public Boolean WasCompilationFailure {
        get => this._wasCompilationFailure;
        set {
            this._wasCompilationFailure = value;
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
            this.WasCompilationSuccess = false;
            this.WasCompilationFailure = true;
            return;
        }

        string filename = file.FileName;
        string json = JsonSerializer.Serialize (this.simulation);

        try {
            File.WriteAllText (filename, json);
        } catch (Exception) {
            this.WasCompilationSuccess = false;
            this.WasCompilationFailure = true;
            return;
        }

        this.WasCompilationSuccess = true;
        this.WasCompilationFailure = false;
    });
}
