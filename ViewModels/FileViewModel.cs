using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using congress_cucuta.Models;
using congress_cucuta.Views;

namespace congress_cucuta.ViewModels;

internal class FileViewModel : ViewModel {
    private bool _wasChoiceFailure = false;

    public Boolean WasChoiceFailure {
        get => this._wasChoiceFailure;
        set {
            this._wasChoiceFailure = value;
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
        //    this.IsChoiceFailed = false;
        //} else {
        //    this.IsChoiceFailed = true;
        //    return;
        //}

        //SimulationModel simulation;

        //try {
        //    string json = File.ReadAllText (file.FileName);
        //    simulation = JsonSerializer.Deserialize<SimulationModel> (json)!;
        //} catch (Exception) {
        //    this.IsChoiceFailed = true;
        //    return;
        //}

        // TODO: Set proper context whenever I figure out how it should work
        SimulationWindow simulationWindow = new () {
            DataContext = new SlideViewModel (),
        };

        simulationWindow.ShowDialog ();
    });
}
