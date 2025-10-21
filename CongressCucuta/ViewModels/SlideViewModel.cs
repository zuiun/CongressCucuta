using CongressCucuta.Data;
using CongressCucuta.Models;

namespace CongressCucuta.ViewModels;

internal class SlideViewModel : ViewModel {
    private string _title = "Title";
    // Intentionally a List, as only setting is intended (no in-place modifications)
    private List<LineViewModel> _description = [];
    // Intentionally a List, as only setting is intended (no in-place modifications)
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

    public void Replace (SlideModel slide, Localisation localisation) {
        Title = slide.Title;
        Description = slide.Description.ConvertAll (l => new LineViewModel (l));

        // SlideBranching or SlideBidirectional
        if (slide.Links.Count > 1) {
            // SlideBranching
            if (slide.IsForward is null) {
                Links = slide.Links.ConvertAll (l => new LinkViewModel (l.Condition.ToString (in localisation), l));
            // SlideBidirectional
            } else {
                Links = [
                    new ("Previous", slide.Links[0]),
                    new ("Next", slide.Links[1]),
                ];
            }
        // SlideConstant
        } else if (slide.Links.Count < 1) {
            Links = [];
        // SlideForward, end SlideBranching, or SlideBackward
        } else {
            // SlideBackward
            if (slide.IsForward is false) {
                Links = [new ("Previous", slide.Links[0])];
            // SlideForward or end SlideBranching
            } else {
                Links = [new ("Next", slide.Links[0])];
            }
        }

        IsContent = slide.IsContent;
        IsSubtitle = ! IsContent;
    }
}
