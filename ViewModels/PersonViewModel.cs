using congress_cucuta.Data;
using System.Collections.ObjectModel;

namespace congress_cucuta.ViewModels;

internal class PersonViewModel (IDType id, string name) : ViewModel, IID {
    internal class RoleGroup : ViewModel {
        private readonly string _name;
        private readonly string _abbreviation;
        public string Name => _name;
        public string Abbreviation => _abbreviation;

        public RoleGroup (string name) {
            string[] words = name.Split (' ');
            string abbreviation = string.Join (string.Empty, words.Select (w => w.First ()));
            
            _name = name;
            _abbreviation = $"[{abbreviation}]";
        }
    }

    internal class ProcedureGroup : ViewModel {

    }

    private string _name = name;
    private ObservableCollection<RoleGroup> _roles = [];
    // TODO needs to fire an event that goes to simulationmodel, which goes to context and changes votes
    private bool _isPass = false;
    private bool _isFail = false;
    private bool _isAbstain = true;
    private bool _canVote = true;
    private bool _canSpeak = true;
    private bool _isInteractable = false;
    private readonly ObservableCollection<ProcedureGroup> _procedures = [];
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
    // These three need to get how many votes the person was worth
    public event EventHandler<bool>? VotingPass = null;
    public event EventHandler<bool>? VotingFail = null;
    public event EventHandler<bool>? VotingAbstain = null;

    public void UpdatePermissions (Permissions permissions) {
        CanVote = permissions.CanVote;
        CanSpeak = permissions.CanSpeak;
    }
}
