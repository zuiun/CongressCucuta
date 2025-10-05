using congress_cucuta.Converters;
using congress_cucuta.Data;
using congress_cucuta.Views;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;

namespace congress_cucuta.ViewModels;

internal class FileViewModel : ViewModel {
    private bool _wasChoiceFailure = false;
    private readonly JsonSerializerOptions _options = new () {
        Converters = { new IDTypeJsonConverter () },
        IncludeFields = true,
    };
    public bool WasChoiceFailure {
        get => _wasChoiceFailure;
        set {
            _wasChoiceFailure = value;
            OnPropertyChanged ();
        }
    }

    public RelayCommand ChooseSimulationCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Simulation files (*.sim)|*.sim",
        };
        bool? result = file.ShowDialog ();

        // TODO: Enable when context is done
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

        SimulationWindow simulationWindow = new () {
            DataContext = new SimulationViewModel (simulation),
        };

        simulationWindow.ShowDialog ();
    });
}
