using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.ViewModels;

internal class FileViewModel : ViewModel {
    private readonly JsonSerializerOptions _options = new () {
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
    private bool _wasChoiceFailure = false;
    private bool _wasCreationFailure = false;
    public bool WasChoiceFailure {
        get => _wasChoiceFailure;
        set {
            _wasChoiceFailure = value;
            OnPropertyChanged ();
        }
    }
    public bool WasCreationFailure {
        get => _wasCreationFailure;
        set {
            _wasCreationFailure = value;
            OnPropertyChanged ();
        }
    }
    public event Action<Simulation>? CreatingSimulation = null;

    public void Reset () {
        WasChoiceFailure = false;
        WasCreationFailure = false;
    }

    public RelayCommand ChooseSimulationCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Simulation files (*.sim)|*.sim",
        };
        bool? result = file.ShowDialog ();

        if (result! == true) {
            WasChoiceFailure = false;
        } else {
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

        WasCreationFailure = false;
        CreatingSimulation?.Invoke (simulation);
    });
}
