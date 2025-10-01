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
}
