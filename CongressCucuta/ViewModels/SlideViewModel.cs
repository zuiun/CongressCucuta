using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;

namespace CongressCucuta.ViewModels;

internal class SlideViewModel : ViewModel, IID {
    private readonly string _title;
    // Intentionally a List, as only setting is intended (no in-place modifications)
    private readonly List<LineViewModel> _description;
    // Intentionally a List, as only setting is intended (no in-place modifications)
    private readonly List<LinkViewModel> _links = [];
    private bool _isContent;
    private bool _isSubtitle;
    public IDType ID { get; }
    public string Title => _title;
    public List<LineViewModel> Description => _description;
    public List<LinkViewModel> Links => _links;
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

    private SlideViewModel (IDType id, string title, List<LineViewModel> description, bool isContent) {
        ID = id;
        _title = title;
        _description = description;
        IsContent = isContent;
        IsSubtitle = ! isContent;

        foreach (LineViewModel line in Description) {
            line.IsContent = IsContent;
            line.IsSubtitle = IsSubtitle;
        }
    }

    public static SlideViewModel Forward (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        slide.Links.Add (new ("Next", new (new AlwaysCondition (), id + 1)));
        return slide;
    }

    public static SlideViewModel Backward (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        slide.Links.Add (new ("Previous", new (new AlwaysCondition (), id - 1)));
        return slide;
    }

    public static SlideViewModel Bidirectional (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        slide.Links.Add (new ("Previous", new (new AlwaysCondition (), id - 1)));
        slide.Links.Add (new ("Next", new (new AlwaysCondition (), id + 1)));
        return slide;
    }

    public static SlideViewModel Branching (
        IDType id,
        string title,
        List<LineViewModel> description,
        List<Link<SlideViewModel>> links,
        ref readonly Localisation localisation,
        bool isContent = true
    ) {
        SlideViewModel slide = new (id, title, description, isContent);

        foreach (Link<SlideViewModel> l in links) {
            slide.Links.Add (new LinkViewModel (l.Condition.ToString (in localisation), l));
        }

        return slide;
    }

    public static SlideViewModel Constant (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        return slide;
    }
}
