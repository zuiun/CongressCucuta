using System.Collections.ObjectModel;
using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

internal class PersonViewModel (IDType id, string name, bool isInteractable) : ViewModel, IID {
    internal class RoleGroup : ViewModel {
        private readonly string _name;
        private readonly string _abbreviation;
        public string Name => _name;
        public string Abbreviation => _abbreviation;

        public RoleGroup (string name) {
            string[] words = name.Split ([' ', '-']);
            string abbreviation = string.Join (string.Empty, words.Select (w => w.First ()));
            
            _name = name;
            _abbreviation = $"[{abbreviation}]";
        }
    }

    private readonly ObservableCollection<RoleGroup> _roles = [];
    private bool _isPass = false;
    private bool _isFail = false;
    private bool _isAbstain = true;
    private bool _canVote = true;
    private byte _votes = 1;
    private bool _canSpeak = true;
    private bool _isInteractable = isInteractable;
    private bool _canDeclare = false;
    public IDType ID => id;
    public string Name => name;
    public ObservableCollection<RoleGroup> Roles => _roles;
    public bool IsPass {
        get => _isPass;
        set {
            if (_isPass != value) {
                _isPass = value;
                OnPropertyChanged ();

                if (IsInteractable) {
                    VotingPass?.Invoke (this, _isPass);
                }
            }
        }
    }
    public bool IsFail {
        get => _isFail;
        set {
            if (_isFail != value) {
                _isFail = value;
                OnPropertyChanged ();

                if (IsInteractable) {
                    VotingFail?.Invoke (this, _isFail);
                }
            }
        }
    }
    public bool IsAbstain {
        get => _isAbstain;
        set {
            if (_isAbstain != value) {
                _isAbstain = value;
                OnPropertyChanged ();

                if (IsInteractable) {
                    VotingAbstain?.Invoke (this, _isAbstain);
                }
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
    public byte Votes {
        get => _votes;
        set {
            _votes = value;
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
    public bool CanDeclare {
        get => _canDeclare;
        set {
            _canDeclare = value;
            OnPropertyChanged ();
        }
    }
    public event EventHandler<bool>? VotingPass = null;
    public event EventHandler<bool>? VotingFail = null;
    public event EventHandler<bool>? VotingAbstain = null;
    public event Action<IDType>? DeclaringProcedure = null;

    public void UpdatePermissions (Permissions permissions) {
        CanVote = permissions.CanVote;
        Votes = permissions.Votes;
        CanSpeak = permissions.CanSpeak;
    }

    public void Reset () {
        IsPass = false;
        IsFail = false;
        IsAbstain = true;
    }

    public RelayCommand DeclareProcedureCommand => new (_ => DeclaringProcedure?.Invoke (ID));
}
