using System.Collections.ObjectModel;

namespace congress_cucuta.Models;

public class SimulationModel (ObservableCollection<SlideModel> slides) {
    public ObservableCollection<SlideModel> Slides { get; } = slides;
}
