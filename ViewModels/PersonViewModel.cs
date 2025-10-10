using congress_cucuta.Data;
using System.Collections.ObjectModel;

namespace congress_cucuta.ViewModels;

// TODO: some way to hide the Declare button if it's not possible. you'd have to make a map in simulationcontext
internal class PersonViewModel (IDType id, string name) : ViewModel, IID {
    internal class RoleGroup : ViewModel, IID {
        private readonly string _name;
        private readonly string _abbreviation;
        public IDType ID { get; }
        public string Name => _name;
        public string Abbreviation => _abbreviation;

        public RoleGroup (IDType id, string name) {
            string[] words = name.Split (' ');
            string abbreviation = string.Join (string.Empty, words.Select (w => w.First ()));
            
            _name = name;
            _abbreviation = $"[{abbreviation}]";
            ID = id;
        }
    }

    private string _name = name;
    private ObservableCollection<RoleGroup> _roles = [];
    private bool _isPass = false;
    private bool _isFail = false;
    private bool _isAbstain = true;
    private bool _canVote = true;
    private bool _canSpeak = true;
    private bool _isInteractable = false;
    public IDType ID => id;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<RoleGroup> Roles {
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

            if (IsInteractable) {
                VotingPass?.Invoke (this, _isPass);
            }
        }
    }
    public bool IsFail {
        get => _isFail;
        set {
            _isFail = value;
            OnPropertyChanged ();

            if (IsInteractable) {
                VotingFail?.Invoke (this, _isFail);
            }
        }
    }
    public bool IsAbstain {
        get => _isAbstain;
        set {
            _isAbstain = value;
            OnPropertyChanged ();

            if (IsInteractable) {
                VotingAbstain?.Invoke (this, _isAbstain);
            }
        }
    }
    public bool IsInteractable {
        get => _isInteractable;
        set {
            _isInteractable = value;

            OnPropertyChanged ();

            if (! _isInteractable) {
                IsPass = false;
                IsFail = false;
                IsAbstain = true;
            }
        }
    }
    public bool CanVote {
        get => _canVote;
        set {
            _canVote = value;
            OnPropertyChanged ();
        }
    }
    public bool CanSpeak {
        get => _canSpeak;
        set {
            _canSpeak = value;
            OnPropertyChanged ();
        }
    }
    public event EventHandler<bool>? VotingPass = null;
    public event EventHandler<bool>? VotingFail = null;
    public event EventHandler<bool>? VotingAbstain = null;
    public event Action<IDType>? DeclaringProcedure = null;

    public void UpdatePermissions (Permissions permissions) {
        CanVote = permissions.CanVote;
        CanSpeak = permissions.CanSpeak;
    }

    public RelayCommand DeclareProcedureCommand => new (_ => DeclaringProcedure?.Invoke (ID));
}
