using System.Collections.ObjectModel;
using congress_cucuta.Data;

namespace congress_cucuta.ViewModels;

internal class ConfirmingProcedureEventArgs (IDType personId, IDType procedureId, bool isManual = false) {
    public IDType PersonID => personId;
    public IDType ProcedureID => procedureId;
    public bool IsManual { get; set; } = isManual;
    public bool IsConfirmed { get; set; } = false;
    public string FailureMessage { get; set; } = string.Empty;
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
    private string _confirmationMessage = string.Empty;
    private bool _isDisplay = true;
    private bool _isManual = false;
    private bool _isConfirmation = false;
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
    public string ConfirmationMessage {
        get => _confirmationMessage;
        set {
            _confirmationMessage = value;
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
    public Action? CloseWindow { get; set; }
    public event Action<ConfirmingProcedureEventArgs>? ConfirmingProcedure;

    public DeclareViewModel (IDType personId, SimulationContext context, Localisation localisation) {
        PersonID = personId;
        Name = context.People[personId].Name;

        foreach (ProcedureDeclared p in context.ProceduresDeclared.Values) {
            ProcedureGroup procedure = new (p.ID, localisation.Procedures[p.ID].Item1);

            if (p.DeclarerIDs.All (d => ! context.PeopleRoles[personId].Contains (d))) {
                procedure.IsActive = false;
                procedure.Error = "Not allowed";
            } else if (context.Context.ProceduresDeclared.Contains (p.ID)) {
                procedure.IsActive = false;
                procedure.Error = "Already declared";
            } else if (p.YieldEffects (0) is Procedure.EffectBundle effects) {
                if (effects.Confirmation is Procedure.Confirmation confirmation) {
                    if (confirmation.Cost is Procedure.Confirmation.CostType.CurrencyValue) {
                        IDType currencyId = context.ChooseCurrencyOwner (personId);

                        if (context.CurrenciesValues[currencyId] < confirmation.Value) {
                            procedure.IsActive = false;
                            procedure.Error = $"Insufficient {localisation.Currencies[currencyId]}";
                        }
                    }
                }
            }

            procedure.DeclaringProcedure += Procedure_DeclaringProcedureEventHandler;
            Procedures.Add (procedure);
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
        } else if (args.IsConfirmed) {
            CloseWindow?.Invoke ();
        } else {
            IsConfirmation = true;
            ConfirmationMessage = args.FailureMessage;
        }
    }

    public RelayCommand ConfirmProcedureCommand => new (_ => {
        ConfirmingProcedureEventArgs args = new (PersonID, _manualId, true);

        ConfirmingProcedure?.Invoke (args);
        CloseWindow?.Invoke ();
    });
}
