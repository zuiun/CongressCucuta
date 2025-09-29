using congress_cucuta.Models;
using System.Collections.ObjectModel;

namespace congress_cucuta.ViewModels;

public class SlideViewModel : ViewModel {
    private string _title = "Title";
    private ObservableCollection<LineModel> _description = [];

    public SlideViewModel () {
        _description.Add (new LineModel ("Line 1", 0, isImportant: true));
        _description.Add (new LineModel ("Line 2", 0, description: "Hover"));
        _description.Add (new LineModel ("Line 3", 0));
        _description.Add (new LineModel ("Line 4", 200));
    }

    public string Title {
        get { return _title; }
        set {
            _title = value;
            OnPropertyChanged ();
        }
    }

    public ObservableCollection<LineModel> Description {
        get { return _description; }
        set {
            _description = value;
            OnPropertyChanged ();
        }
    }
}
