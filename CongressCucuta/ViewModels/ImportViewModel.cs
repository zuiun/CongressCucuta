using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using CongressCucuta.Converters;
using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class ImportViewModel : ViewModel {
    private readonly JsonSerializerOptions _options = new () {
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
    private bool _wasChoiceFailure = false;
    public bool WasChoiceFailure {
        get => _wasChoiceFailure;
        set {
            _wasChoiceFailure = value;
            OnPropertyChanged ();
        }
    }
    public event Action<Simulation>? CreatingSimulation = null;

    public void Reset () {
        WasChoiceFailure = false;
    }

    public RelayCommand ChooseSimulationCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Simulation files (*.sim)|*.sim",
        };

        WasChoiceFailure = false;

        bool? result = file.ShowDialog ();

        if (result != true) {
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

        CreatingSimulation?.Invoke (simulation);
    });
}
