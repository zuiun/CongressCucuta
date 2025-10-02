using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class SimulationViewModel : ViewModel {
    private readonly SimulationModel _simulation; // TODO: This needs to be set properly
    private readonly SlideViewModel _slide = new ();
    private readonly string _state = "State"; // TODO: Get this from Simulation
    public SimulationModel Simulation => _simulation;
    public SlideViewModel Slide => _slide;
    public string State => _state;

    // TODO: This is only for testing!
    // TODO: After creation, we need to Replace the first slide so that everything can begin
    public SimulationViewModel () {

        _simulation = new ();
        _state = _simulation.State;
        SlideModel slide = _simulation.Slides[0];
        _slide.Replace (in slide);
    }
    
    public RelayCommand<Link<SlideModel>> SwitchSlideCommand => new (
        l => {
            IDType? result = _simulation.ResolveLink (l);
                    
            if (result is not null) {
                IDType slideIdx = (IDType) result!;
                SlideModel slide = _simulation.Slides[slideIdx];

                Slide.Replace (in slide);
                Simulation.SlideCurrentIdx = slideIdx;
            }
        },
        l => _simulation.EvaluateLink (l)
    );
}
