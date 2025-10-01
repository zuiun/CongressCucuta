namespace congress_cucuta.ViewModels;

internal class SlideViewModel : ViewModel {
    private string _title = "Title";
    private List<LineViewModel> _description = [];

    public string Title {
        get => _title;
        set {
            _title = value;
            OnPropertyChanged ();
        }
    }

    public List<LineViewModel> Description {
        get => _description;
        set {
            _description = value;
            OnPropertyChanged ();
        }
    }
}
