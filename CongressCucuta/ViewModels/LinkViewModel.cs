using System.Windows.Input;
using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

internal class LinkViewModel (string name, Link<SlideViewModel> link, Key key = Key.None) : ViewModel {
    private Key _key = key;
    public string Name => name;
    public Link<SlideViewModel> Link => link;
    public Key Key {
        get => _key;
        set {
            _key = value;
            OnPropertyChanged ();
        }
    }
}
