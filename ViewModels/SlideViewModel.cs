namespace congress_cucuta.ViewModels;

internal class SlideViewModel : ViewModel {
    private string _title = "Title";
    private List<LineViewModel> _description = [];

    public string Title {
        get => this._title;
        set {
            this._title = value;
            OnPropertyChanged ();
        }
    }

    public List<LineViewModel> Description {
        get => this._description;
        set {
            this._description = value;
            OnPropertyChanged ();
        }
    }
}
