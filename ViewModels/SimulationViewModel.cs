using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class SimulationViewModel : ViewModel {
    private readonly SimulationModel _simulation;
    private readonly SlideViewModel _slide = new ();
    private readonly string _state;
    public SimulationModel Simulation => _simulation;
    public SlideViewModel Slide => _slide;
    public string State => _state;

    public SimulationViewModel (Simulation simulation) {
        _simulation = new (simulation);
        _state = _simulation.Localisation.State;

        SlideModel slide = _simulation.Slides[0];

        _slide.Replace (in slide, _simulation.Localisation);
    }

    public RelayCommand<Link<SlideModel>> SwitchSlideCommand => new (
        l => {
            IDType? result = _simulation.ResolveLink (l);
            
            if (result is not null) {
                IDType slideIdx = (IDType) result!;
                SlideModel slide = _simulation.Slides[slideIdx];

                Slide.Replace (in slide, _simulation.Localisation);
                Simulation.SlideCurrentIdx = slideIdx;
            }
        },
        l => _simulation.EvaluateLink (l)
    );
}
