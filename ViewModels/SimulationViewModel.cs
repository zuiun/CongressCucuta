using congress_cucuta.Data;
using congress_cucuta.Models;
using congress_cucuta.Views;

namespace congress_cucuta.ViewModels;

internal class SimulationViewModel : ViewModel {
    private readonly SimulationModel _simulation;
    private readonly SlideViewModel _slide = new ();
    private readonly ContextViewModel _context;
    private readonly string _state;
    private readonly Dictionary<IDType, string> _proceduresEffects = [];
    public SlideViewModel Slide => _slide;
    public ContextViewModel Context => _context;
    public string State => _state;

    public SimulationViewModel (Simulation simulation) {
        _simulation = new (simulation);
        _simulation.CompletingElection += Simulation_CompletingElectionEventHandler;
        _simulation.Context.ModifiedProcedures += Context_ModifiedProceduresEventHandler;
        _state = _simulation.Localisation.State;
        _context = new (in _simulation);
        _context.Voting += _simulation.Context_VotingEventHandler;
        _context.DeclaringProcedure += Context_DeclaringProcedureEventHandler;

        Localisation localisation = _simulation.Localisation;

        foreach (ProcedureImmediate pi in _simulation.Context.ProceduresGovernmental.Values) {
            string effect = pi.ToString (in simulation, in localisation);
            ContextViewModel.ProcedureGroup procedure = new (
                pi.ID,
                _simulation.Localisation.Procedures[pi.ID].Item1,
                effect
            );

            _proceduresEffects[pi.ID] = effect;
            _context.ProceduresGovernmental.Add (procedure);
        }

        foreach (ProcedureTargeted pt in _simulation.Context.ProceduresSpecial.Values) {
            string effect = pt.ToString (in simulation, in localisation);

            if (pt.IsActiveStart) {
                ContextViewModel.ProcedureGroup procedure = new (
                    pt.ID,
                    _simulation.Localisation.Procedures[pt.ID].Item1,
                    effect
                );

                _context.ProceduresSpecial.Add (procedure);
            }

            _proceduresEffects[pt.ID] = effect;
        }

        foreach (ProcedureDeclared pd in _simulation.Context.ProceduresDeclared.Values) {
            string effect = pd.ToString (in simulation, in localisation);
            ContextViewModel.ProcedureGroup procedure = new (
                pd.ID,
                _simulation.Localisation.Procedures[pd.ID].Item1,
                effect
            );

            _context.ProceduresDeclared.Add (procedure);
            _proceduresEffects[pd.ID] = effect;
        }

        _context.Sort ();

        SlideModel slide = _simulation.Slides[0];

        _slide.Replace (in slide, _simulation.Localisation);
    }

    private void Simulation_CompletingElectionEventHandler (CompletingElectionEventArgs e) {
        ElectionViewModel election = new (e);
        ElectionWindow window = new () {
            DataContext = election,
        };

        // yeah ok fight me
        election.CompletedElection += window.Election_CompletedElectionEventHandler;
        election.CloseWindow = window.Close;
        window.ShowDialog ();
        e.PeopleRolesNew = election.PeopleRolesNew;
        e.PeopleFactionsNew = election.PeopleFactionsNew;
    }

    private void Context_DeclaringProcedureEventHandler (IDType e) {
        DeclareViewModel declare = new (
            e,
            _simulation.Context,
            _simulation.Localisation
        );

        declare.ConfirmingProcedure += Declare_ConfirmingProcedureEventHandler;

        DeclareWindow window = new () {
            DataContext = declare,
        };

        window.ShowDialog ();
    }

    private void Declare_ConfirmingProcedureEventHandler (ConfirmingProcedureEventArgs e) {
        if (e.IsManual) {
            _simulation.DeclareProcedure (e.PersonID, e.ProcedureID);
        } else {
            (bool, string)? result = _simulation.Context.TryConfirmProcedure (e.PersonID, e.ProcedureID);

            if (result is (bool isConfirmed, string failureMessage)) {
                e.IsConfirmed = isConfirmed;
                e.FailureMessage = failureMessage;

                if (isConfirmed) {
                    _simulation.DeclareProcedure (e.PersonID, e.ProcedureID);
                }
            } else {
                e.IsManual = true;
            }
        }
    }

    private void Context_ModifiedProceduresEventHandler (HashSet<ProcedureTargeted> e) {
        _context.ProceduresSpecial.Clear ();

        foreach (ProcedureTargeted pt in e) {
            ContextViewModel.ProcedureGroup procedure = new (
                pt.ID,
                _simulation.Localisation.Procedures[pt.ID].Item1,
                _proceduresEffects[pt.ID]
            );

            _context.ProceduresSpecial.Add (procedure);
        }

        _context.Sort ();
    }

    public void InitialisePeople (List<Person> people) => _simulation.InitialisePeople (people);

    public RelayCommand<Link<SlideModel>> SwitchSlideCommand => new (
        l => {
            IDType? result = _simulation.ResolveLink (l);
            
            if (result is not null) {
                IDType slideIdx = (IDType) result!;
                SlideModel slide = _simulation.Slides[slideIdx];

                if (l.YieldBallotVote () is bool isPass) {
                    _simulation.VoteBallot (isPass);
                }

                _slide.Replace (in slide, _simulation.Localisation);
                _simulation.SlideCurrentIdx = slideIdx;
            }
        },
        l => _simulation.EvaluateLink (l)
    );
}
