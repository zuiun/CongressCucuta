using System.Collections.ObjectModel;

namespace congress_cucuta.ViewModels;

internal class PersonViewModel : ViewModel {
    private string _name = "Name";
    private ObservableCollection<string> _roles = [];
    // TODO Permissions
    private bool _isPass = false;
    private bool _isFail = false;
    private bool _isAbstain = true;
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
    public bool IsPass {
        get => _isPass;
        set {
            _isPass = value;
            OnPropertyChanged ();
        }
    }
    public bool IsFail {
        get => _isFail;
        set {
            _isFail = value;
            OnPropertyChanged ();
        }
    }
    public bool IsAbstain {
        get => _isAbstain;
        set {
            _isAbstain = value;
            OnPropertyChanged ();
        }
    }
}
