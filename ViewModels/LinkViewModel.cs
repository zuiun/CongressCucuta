using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class LinkViewModel : ViewModel {
    private string _name = "Link";
    private Link<SlideModel> _link = new (new AlwaysCondition (), 0);

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
