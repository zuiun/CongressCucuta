using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

internal enum Shortcut {
    Left,
    Up,
    Down,
    Right,
    None,
}

internal class LinkViewModel (string name, Link<SlideViewModel> link, Shortcut shortcut = Shortcut.None) : ViewModel {
    private Shortcut _shortcut = shortcut;
    public string Name => name;
    public Link<SlideViewModel> Link => link;
    public Shortcut Shortcut {
        get => _shortcut;
        set {
            _shortcut = value;
            OnPropertyChanged ();
        }
    }
}
