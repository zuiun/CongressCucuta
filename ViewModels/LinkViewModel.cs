using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class LinkViewModel (string name, Link<SlideModel> link) : ViewModel {
    private string _name = name;
    private Link<SlideModel> _link = link;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public Link<SlideModel> Link {
        get => _link;
        set {
            _link = value;
            OnPropertyChanged ();
        }
    }
}
