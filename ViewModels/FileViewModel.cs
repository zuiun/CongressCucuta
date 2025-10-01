using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using congress_cucuta.Models;
using congress_cucuta.Views;

namespace congress_cucuta.ViewModels;

internal class FileViewModel : ViewModel {
    private bool _wasChoiceFailure = false;

    public bool WasChoiceFailure {
        get => _wasChoiceFailure;
        set {
            _wasChoiceFailure = value;
            OnPropertyChanged ();
        }
    }

    public RelayCommand ChooseSimulationCommand => new (_ => {
        OpenFileDialog file = new () {
            Filter = "Simulation files (*.sim)| *.sim",
        };
        bool? result = file.ShowDialog ();

        // TODO: Enable when context is done
        //if (result! == true) {
        //    IsChoiceFailed = false;
        //} else {
        //    IsChoiceFailed = true;
        //    return;
        //}

        //SimulationModel simulation;

        //try {
        //    string json = File.ReadAllText (file.FileName);
        //    simulation = JsonSerializer.Deserialize<Simulation> (json)!;
        //} catch (Exception) {
        //    IsChoiceFailed = true;
        //    return;
        //}

        // TODO: now convert the Simulation to a SimulationModel, then pass it to SimulationViewModel

        // TODO: Set proper context whenever I figure out how it should work
        SimulationWindow simulationWindow = new () {
            // TODO: this should be a SimulationViewModel
            // TODO: SimulationWindow contains a SlideView, which should contain the SlideViewModel
            DataContext = new SimulationViewModel (),
        };

        simulationWindow.ShowDialog ();
    });
}
