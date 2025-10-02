using System.Collections.ObjectModel;

namespace congress_cucuta.ViewModels;

internal enum Vote {
    Pass,
    Fail,
    Abstain,
}

internal class PersonViewModel : ViewModel {
    private string _name = "Name";
    private ObservableCollection<string> _roles = [];
    private Vote _vote = Vote.Abstain;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<string> Roles {
        get => _roles;
        set {
            _roles = value;
            OnPropertyChanged ();
        }
    }
    public Vote Vote {
        get => _vote;
        set {
            _vote = value;
            OnPropertyChanged ();
        }
    }
}
