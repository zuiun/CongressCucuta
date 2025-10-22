using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class LinkViewModel (string name, Link<SlideViewModel> link) : ViewModel {
    private string _name = name;
    private Link<SlideViewModel> _link = link;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public Link<SlideViewModel> Link {
        get => _link;
        set {
            _link = value;
            OnPropertyChanged ();
        }
    }
}
