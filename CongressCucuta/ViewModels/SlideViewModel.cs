using System.Diagnostics;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;

namespace CongressCucuta.ViewModels;

internal class SlideViewModel : ViewModel, IID {
    private readonly string _title;
    // Intentionally a List, as only setting is intended (no in-place modifications)
    private List<LineViewModel> _description;
    // Intentionally a List, as only setting is intended (no in-place modifications)
    private readonly List<LinkViewModel> _links = [];
    private bool _isContent;
    private bool _isSubtitle;
    public IDType ID { get; }
    public string Title => _title;
    public List<LineViewModel> Description {
        get => _description;
        set {
            _description = value;
            OnPropertyChanged ();
        }
    }
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

        slide.Links.Add (new ("Next", new (new AlwaysCondition (), id + 1), Shortcut.Right));
        return slide;
    }

    public static SlideViewModel Backward (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        slide.Links.Add (new ("Previous", new (new AlwaysCondition (), id - 1), Shortcut.Left));
        return slide;
    }

    public static SlideViewModel Bidirectional (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        slide.Links.Add (new ("Previous", new (new AlwaysCondition (), id - 1), Shortcut.Left));
        slide.Links.Add (new ("Next", new (new AlwaysCondition (), id + 1), Shortcut.Right));
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

        if (links.Count == 0) {
            throw new UnreachableException ();
        } else if (links.Count == 1) {
            slide.Links[0].Shortcut = Shortcut.Right;
        } else if (links.Count == 2) {
            slide.Links[0].Shortcut = Shortcut.Left;
            slide.Links[1].Shortcut = Shortcut.Right;
        } else if (links.Count == 3) {
            slide.Links[0].Shortcut = Shortcut.Left;
            slide.Links[1].Shortcut = Shortcut.Up;
            slide.Links[2].Shortcut = Shortcut.Right;
        } else {
            slide.Links[0].Shortcut = Shortcut.Left;
            slide.Links[1].Shortcut = Shortcut.Up;
            slide.Links[links.Count - 2].Shortcut = Shortcut.Down;
            slide.Links[links.Count - 1].Shortcut = Shortcut.Right;
        }

        return slide;
    }

    public static SlideViewModel Constant (IDType id, string title, List<LineViewModel> description, bool isContent = true) {
        SlideViewModel slide = new (id, title, description, isContent);

        return slide;
    }

    public Link<SlideViewModel>? FindLink (Shortcut shortcut) {
        LinkViewModel? link = _links.Find (l => l.Shortcut == shortcut);

        return (link is LinkViewModel l) ? l.Link : null;
    }
}
