using CongressCucuta.Data;
using CongressCucuta.Models;
using CongressCucuta.Views;
using System.Diagnostics;

namespace CongressCucuta.ViewModels;

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
        DeclareWindow window = new () {
            DataContext = declare,
        };

        declare.ConfirmingProcedure += Declare_ConfirmingProcedureEventHandler;
        window.ShowDialog ();
    }

    private void Declare_ConfirmingProcedureEventHandler (ConfirmingProcedureEventArgs e) {
        if (e.IsManual) {
            e.IsManual = false;
            e.IsConfirmed = true;
            e.Message = "Success";
            _simulation.DeclareProcedure (e.PersonID, e.ProcedureID);
        } else {
            SimulationContext.ConfirmationResult result = _simulation.Context.TryConfirmProcedure (e.PersonID, e.ProcedureID);

            switch (result.Cost) {
                case Procedure.Confirmation.CostType.Always:
                    e.Message = "Success";
                    break;
                // case Procedure.Confirmation.CostType.DivisionChamber:
                case Procedure.Confirmation.CostType.CurrencyValue: {
                    if (result.IsConfirmed == true) {
                        (IDType currencyId, sbyte _) = result.Currency ?? default;

                        e.Message = $"Success: Spent {result.Value!} {_simulation.Localisation.Currencies[currencyId]}";
                    } else {
                        throw new UnreachableException ();
                    }

                    break;
                }
                case Procedure.Confirmation.CostType.DiceValue:
                    if (result.IsConfirmed == true) {
                        e.Message = $"Success: Rolled {result.DiceDeclarer!}";
                    } else {
                        e.Message = $"Failure: Rolled {result.DiceDeclarer!}, but needed at least {result.Value!}";
                    }

                    break;
                case Procedure.Confirmation.CostType.DiceCurrency: {
                    (IDType currencyId, sbyte currency) = result.Currency ?? default;

                    if (result.IsConfirmed == true) {
                        e.Message = $"Success: Rolled and spent {result.DiceDeclarer!} {_simulation.Localisation.Currencies[currencyId]}";
                    } else {
                        e.Message = $"Failure: Rolled {result.DiceDeclarer!}, but only had {currency} {_simulation.Localisation.Currencies[currencyId]}";
                    }

                    break;
                }
                case Procedure.Confirmation.CostType.DiceAdversarial: {
                    if (result.IsConfirmed == true) {
                        if (result.Currency is (IDType currencyId, sbyte _)) {
                            e.Message = $"Success: Rolled and spent {result.DiceDeclarer!} {_simulation.Localisation.Currencies[currencyId]}, while defender rolled {result.DiceDefender!}";
                        } else {
                            e.Message = $"Success: Rolled {result.DiceDeclarer!}, while defender rolled {result.DiceDefender!}";
                        }
                    } else {
                        if (result.Currency is (IDType currencyId, sbyte currency)) {
                            if (result.DiceDefender is null) {
                                e.Message = $"Failure: Rolled {result.DiceDeclarer!}, but only had {currency} {_simulation.Localisation.Currencies[currencyId]}";
                            } else {
                                e.Message = $"Failure: Rolled and spent {result.DiceDeclarer!} {_simulation.Localisation.Currencies[currencyId]}, but defender rolled {result.DiceDefender!}";
                            }
                        } else {
                            if (result.DiceDefender is null) {
                                throw new UnreachableException ();
                            } else {
                                e.Message = $"Failure: Rolled and spent {result.DiceDeclarer!}, but defender rolled {result.DiceDefender!}";
                            }
                        }
                    }

                    break;
                }
            }

            if (result.IsConfirmed is bool isConfirmed) {
                e.IsConfirmed = isConfirmed;

                if (isConfirmed) {
                    bool? vote = _simulation.DeclareProcedure (e.PersonID, e.ProcedureID);

                    if (vote is bool isPass) {
                        LinkViewModel link = isPass ? _slide.Links[0] : _slide.Links[1];
                        IDType slideIdx = link.Link.TargetID;
                        SlideModel slide = _simulation.Slides[slideIdx];

                        _slide.Replace (in slide, _simulation.Localisation);
                        _simulation.SlideCurrentIdx = slideIdx;
                    }
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
