using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class SlideViewModel : ViewModel {
    private string _title = "Title";
    private List<LineViewModel> _description = [];
    private List<LinkViewModel> _links = [];
    private bool _isContent = true;
    private bool _isSubtitle = false;
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
    public bool IsContent {
        get => _isContent;
        set {
            _isContent = value;
            OnPropertyChanged ();
        }
    }
    public bool IsSubtitle {
        get => _isSubtitle;
        set {
            _isSubtitle = value;
            OnPropertyChanged ();
        }
    }

    public void Replace (ref readonly SlideModel slide) {
        Title = slide.Title;
        Description = slide.Description.ConvertAll (l => new LineViewModel (l));

        // SlideBranching
        if (slide.Links.Count > 1) {
            Links = slide.Links.ConvertAll (l => new LinkViewModel (l.ToString (), l));
        // SlideConstant
        } else if (slide.Links.Count < 1) {
            Links = [];
        // SlideLinear or end SlideBranching
        } else {
            Links = [new ("Next", slide.Links[0])];
        }

        IsContent = slide.IsContent;
        IsSubtitle = ! IsContent;
    }
}
