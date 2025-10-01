using System.Windows;
using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class SimulationViewModel () : ViewModel {
    private SlideViewModel _slide = new ();

    public SlideViewModel Slide {
        get => _slide;
        set {
            _slide = value;
            OnPropertyChanged ();
        }
    }

    public RelayCommand<Link<SlideModel>> SwitchSlideCommand => new (
        l => {
            MessageBox.Show ($"You clicked {l.TargetID}");
            // TODO: switch slides
        },
        l => {
            // TODO: validate
            return true;
        }
    );
}
