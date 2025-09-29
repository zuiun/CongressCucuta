namespace congress_cucuta.ViewModels;

public class SlideViewModel : ViewModel {
    private string _title = "Title";
    private List<LineViewModel> _description = [];

    public string Title {
        get { return _title; }
        set {
            _title = value;
            OnPropertyChanged ();
        }
    }

    public List<LineViewModel> Description {
        get { return _description; }
        set {
            _description = value;
            OnPropertyChanged ();
        }
    }
}
