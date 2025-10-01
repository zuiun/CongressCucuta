using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class SlideViewModel : ViewModel {
    private string _title = "Title";
    private List<LineViewModel> _description = [];
    private List<LinkViewModel> _links = [
        new () {
            Name = "bruh!",
            Link = new Link<SlideModel> (new AlwaysCondition (), 0)
        },
        new () {
            Name = "???",
            Link = new Link<SlideModel> (new AlwaysCondition (), 1)
        }
    ];

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

    public List<LinkViewModel> Links {
        get => _links;
        set {
            _links = value;
            OnPropertyChanged ();
        }
    }
}
