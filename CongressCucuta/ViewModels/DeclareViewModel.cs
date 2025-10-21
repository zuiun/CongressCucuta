using System.Collections.ObjectModel;
using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.ViewModels;

internal class ConfirmingProcedureEventArgs (IDType personId, IDType procedureId, bool isManual = false) {
    public IDType PersonID => personId;
    public IDType ProcedureID => procedureId;
    public bool IsManual { get; set; } = isManual;
    public bool IsConfirmed { get; set; } = false;
    public string Message { get; set; } = string.Empty;
}

internal class DeclareViewModel : ViewModel {
    internal class ProcedureGroup (IDType id, string name): ViewModel, IID {
        private bool _isActive = true;
        public IDType ID => id;
        public string Name => name;
        public bool IsActive {
            get => _isActive;
            set {
                _isActive = value;
                OnPropertyChanged ();
            }
        }
        public string Error { get; set; } = string.Empty;
        public event Action<IDType>? DeclaringProcedure;

        public RelayCommand DeclareProcedureCommand => new (
            _ => DeclaringProcedure?.Invoke (ID),
            _ => _isActive
        );
    }

    private ObservableCollection<ProcedureGroup> _procedures = [];
    private string _message = string.Empty;
    private bool _isDisplay = true;
    private bool _isManual = false;
    private bool _isConfirmation = false;
    private bool _isSuccess = false;
    private IDType _manualId;
    public IDType PersonID { get; set; }
    public string Name { get; }
    public ObservableCollection<ProcedureGroup> Procedures {
        get => _procedures;
        set {
            _procedures = value;
            OnPropertyChanged ();
        }
    }
    public string Message {
        get => _message;
        set {
            _message = value;
            OnPropertyChanged ();
        }
    }
    public bool IsDisplay {
        get => _isDisplay;
        set {
            _isDisplay = value;
            OnPropertyChanged ();
        }
    }
    public bool IsManual {
        get => _isManual;
        set {
            _isManual = value;
            OnPropertyChanged ();
        }
    }
    public bool IsConfirmation {
        get => _isConfirmation;
        set {
            _isConfirmation = value;
            OnPropertyChanged ();
        }
    }
    public bool IsSuccess {
        get => _isSuccess;
        set {
            _isSuccess = value;
            OnPropertyChanged ();
        }
    }
    public event Action<ConfirmingProcedureEventArgs>? ConfirmingProcedure;

    public DeclareViewModel (IDType personId, SimulationContext context, Localisation localisation) {
        PersonID = personId;
        Name = context.People[personId].Name;

        foreach (ProcedureDeclared p in context.ProceduresDeclared.Values) {
            ProcedureGroup procedure = new (p.ID, localisation.Procedures[p.ID].Item1);

            procedure.DeclaringProcedure += Procedure_DeclaringProcedureEventHandler;
            Procedures.Add (procedure);

            if (p.DeclarerIDs.Length > 0 && p.DeclarerIDs.All (d => ! context.PeopleRoles[personId].Contains (d))) {
                procedure.IsActive = false;
                procedure.Error = "Not allowed";
                continue;
            }

            if (context.Context.ProceduresDeclared.Contains (p.ID)) {
                procedure.IsActive = false;
                procedure.Error = "Already declared";
                continue;
            }
            
            if (
                p.YieldEffects (0) is Procedure.EffectBundle effects
                && effects.Confirmation is Confirmation confirmation
                && confirmation.Type is Confirmation.ConfirmationType.CurrencyValue
            ) {
                IDType currencyId = context.ChooseCurrencyOwner (personId);

                if (context.CurrenciesValues[currencyId] < confirmation.Value) {
                    procedure.IsActive = false;
                    procedure.Error = $"Insufficient {localisation.Currencies[currencyId]}";
                }
            }
        }

        Procedures = [.. Procedures.OrderBy (p => p.ID)];
    }

    private void Procedure_DeclaringProcedureEventHandler (IDType e) {
        ConfirmingProcedureEventArgs args = new (PersonID, e);

        ConfirmingProcedure?.Invoke (args);
        IsDisplay = false;

        if (args.IsManual) {
            _manualId = e;
            IsManual = true;
        } else {
            IsSuccess = args.IsConfirmed;
            IsConfirmation = true;
            Message = args.Message;
        }
    }

    public RelayCommand ConfirmProcedureCommand => new (_ => {
        ConfirmingProcedureEventArgs args = new (PersonID, _manualId, true);

        ConfirmingProcedure?.Invoke (args);
        IsDisplay = false;
        IsManual = args.IsManual;
        IsSuccess = args.IsConfirmed;
        IsConfirmation = true;
        Message = args.Message;
    });
}
