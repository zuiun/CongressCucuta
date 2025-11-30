using System.Diagnostics;
using System.Windows.Input;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Generators;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Views;

namespace CongressCucuta.ViewModels;

internal class SimulationViewModel : ViewModel {
    private readonly Simulation _raw;
    private readonly SimulationContext _simulation;
    private readonly Localisation _localisation;
    private IDType _slideCurrentIdx = 0;
    private readonly IDType _slideTitleIdx;
    private readonly Dictionary<IDType, IDType> _ballotIdxsIds;
    private bool _isBallot = false;
    private SlideViewModel _slide;
    private readonly ContextViewModel _context;
    private readonly IWindow<ElectionWindow, ElectionViewModel> _election;
    private readonly IWindow<DeclareWindow, DeclareViewModel> _declare;
    public Localisation Localisation => _localisation;
    public List<SlideViewModel> Slides { get; } = [];
    public IDType SlideCurrentIdx {
        get => _slideCurrentIdx;
        set {
            _slideCurrentIdx = value;
            Slide = Slides[_slideCurrentIdx];

            if (_slideCurrentIdx == _slideTitleIdx) {
                _simulation.StartSetup ();
            } else if (_ballotIdxsIds.TryGetValue (_slideCurrentIdx, out IDType ballotId)) {
                _simulation.BallotCurrentID = ballotId;
                _isBallot = true;
                _simulation.IsBallot = true;
                _simulation.StartBallot ();
                Context.StartBallot (
                    _simulation.Context.CalculateVotesPassThreshold (),
                    _simulation.Context.CalculateVotesFailThreshold (),
                    _simulation.Context.CalculateVotesPass (),
                    _simulation.Context.CalculateVotesFail (),
                    _simulation.Context.CalculateVotesAbstain ()
                );
            } else if (_isBallot) {
                _isBallot = false;
                _simulation.IsBallot = false;
                _context.EndBallot ();
            }
        }
    }
    public SlideViewModel Slide {
        get => _slide;
        set {
            _slide = value;
            OnPropertyChanged ();
        }
    }
    public ContextViewModel Context => _context;
    public string State { get; }

    public SimulationViewModel (
        Simulation simulation,
        IWindow<ElectionWindow, ElectionViewModel>? election = null,
        IWindow<DeclareWindow, DeclareViewModel>? declare = null,
        IGenerator? generator = null
    ) {
        _raw = simulation;
        _simulation = new (simulation, generator);
        _localisation = simulation.Localisation;
        State = _localisation.State;
        _context = new (_simulation, simulation, in _localisation);
        _election = election ?? new ModalWindow<ElectionWindow, ElectionViewModel> ((e) => new ElectionWindow () {
            DataContext = e,
        });
        _declare = declare ?? new ModalWindow<DeclareWindow, DeclareViewModel> ((d) => new DeclareWindow () {
            DataContext = d,
        });
        _simulation.PreparingElection += Simulation_PreparingElectionEventHandler;
        _simulation.ReplacingParty += Simulation_ReplacingPartyEventHandler;
        _context.Voting += Context_VotingEventHandler;
        _context.DeclaringProcedure += Context_DeclaringProcedureEventHandler;

        IDType slideCurrentIdx = GenerateSlidesIntroduction ();

        slideCurrentIdx = GenerateSlidesProcedures (simulation, slideCurrentIdx);
        slideCurrentIdx = GenerateSlideRoles (simulation, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesFactions (simulation, slideCurrentIdx);
        (slideCurrentIdx, _slideTitleIdx) = GenerateSlidesTitles (simulation, slideCurrentIdx);
        (slideCurrentIdx, _ballotIdxsIds) = GenerateSlidesBallots (simulation, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesResults (simulation, slideCurrentIdx);
        GenerateSlidesEnd (simulation, slideCurrentIdx);
        _slide = Slides[0];
    }

    private IDType GenerateSlidesIntroduction () {
        IDType slideCurrentIdx = 0;
        SlideViewModel slideIntro = SlideViewModel.Forward (slideCurrentIdx, _localisation.State, [_localisation.Government], false);

        ++ slideCurrentIdx;
        Slides.Add (slideIntro);

        SlideViewModel slideContext = SlideViewModel.Bidirectional (slideCurrentIdx, "Context", [.. _localisation.Context]);

        ++ slideCurrentIdx;
        Slides.Add (slideContext);
        return slideCurrentIdx;
    }

    private IDType GenerateSlidesProcedures (Simulation simulation, IDType slideCurrentIdx) {
        void GenerateSlideProcedure (string title, Procedure procedure) {
            string procedureFull = procedure.ToString (simulation, in _localisation);
            string[] procedureSplit = procedureFull.Split ('\n');
            LineViewModel line = new (procedureSplit[0], description: _localisation.Procedures[procedure.ID].Item2);
            SlideViewModel slideProcedure = SlideViewModel.Bidirectional (
                slideCurrentIdx,
                title,
                [line, .. procedureSplit[1 ..]]
            );

            ++ slideCurrentIdx;
            Slides.Add (slideProcedure);
        }

        for (byte i = 0; i < simulation.ProceduresGovernmental.Count; ++i) {
            string title = i > 0 ? "Governmental Procedures (cont.)" : "Governmental Procedures";
            Procedure procedure = simulation.ProceduresGovernmental[i];

            GenerateSlideProcedure (title, procedure);
        }

        for (byte i = 0; i < simulation.ProceduresSpecial.Count; ++i) {
            string title = i > 0 ? "Special Procedures (cont.)" : "Special Procedures";
            Procedure procedure = simulation.ProceduresSpecial[i];

            if (procedure.IsActiveStart) {
                GenerateSlideProcedure (title, procedure);
            }
        }

        for (byte i = 0; i < simulation.ProceduresDeclared.Count; ++i) {
            string title = i > 0 ? "Declared Procedures (cont.)" : "Declared Procedures";
            Procedure procedure = simulation.ProceduresDeclared[i];

            GenerateSlideProcedure (title, procedure);
        }

        return slideCurrentIdx;
    }

    private IDType GenerateSlideRoles (Simulation simulation, IDType slideCurrentIdx) {
        List<string> roles = [];
        string speaker = _localisation.Speaker;
        string member = _localisation.Roles[Role.MEMBER].Item1;

        roles.Add (speaker);
        roles.Add (member);

        if (_localisation.Roles.TryGetValue (Role.HEAD_GOVERNMENT, out (string, string) headGovernment)) {
            roles.Add (headGovernment.Item1);
        }

        if (_localisation.Roles.TryGetValue (Role.HEAD_STATE, out (string, string) headState)) {
            roles.Add (headState.Item1);
        }

        if (_localisation.Roles.TryGetValue (Role.RESERVED_1, out (string, string) reserved1)) {
            roles.Add (reserved1.Item1);
        }

        if (_localisation.Roles.TryGetValue (Role.RESERVED_2, out (string, string) reserved2)) {
            roles.Add (reserved2.Item1);
        }

        if (_localisation.Roles.TryGetValue (Role.RESERVED_3, out (string, string) reserved3)) {
            roles.Add (reserved3.Item1);
        }

        if (simulation.Parties.Count > 0) {
            if (_localisation.Roles.TryGetValue (Role.LEADER_PARTY, out (string, string) leaderParty)) {
                List<string> leadersParties = [];

                roles.Add ($"{leaderParty.Item2}:");

                foreach (Faction party in simulation.Parties) {
                    string leaderPartyIndividual = _localisation.Roles[party.ID].Item1;

                    leadersParties.Add (leaderPartyIndividual);
                }

                roles.Add (StringLineFormatter.Indent (string.Join (", ", leadersParties), 1));
            }
        }

        if (simulation.Regions.Count > 0) {
            if (_localisation.Roles.TryGetValue (Role.LEADER_REGION, out (string, string) leaderRegion)) {
                List<string> leadersRegions = [];

                roles.Add ($"{leaderRegion.Item2}:");

                foreach (Faction region in simulation.Regions) {
                    string leaderRegionIndividual = _localisation.Roles[region.ID].Item1;

                    leadersRegions.Add (leaderRegionIndividual);
                }

                roles.Add (StringLineFormatter.Indent (string.Join (", ", leadersRegions), 1));
            }
        }

        SlideViewModel slideRoles = SlideViewModel.Bidirectional (slideCurrentIdx, "Roles", [.. roles]);

        ++ slideCurrentIdx;
        Slides.Add (slideRoles);
        return slideCurrentIdx;
    }

    private IDType GenerateSlidesFactions (Simulation simulation, IDType slideCurrentIdx) {
        List<string> regions = [];

        foreach (Faction region in simulation.Regions) {
            if (region.IsActiveStart) {
                (string name, string[] description) = _localisation.Regions[region.ID];

                regions.Add (name);
                regions.AddRange (description);
            }
        }

        if (regions.Count > 0) {
            SlideViewModel slideRegions = SlideViewModel.Bidirectional (slideCurrentIdx, _localisation.Region.Item2, [.. regions]);

            ++ slideCurrentIdx;
            Slides.Add (slideRegions);
        }

        List<string> parties = [];

        foreach (Faction party in simulation.Parties) {
            if (party.IsActiveStart) {
                parties.Add (_localisation.GetFactionAndAbbreviation (party.ID));
                parties.AddRange (_localisation.Parties[party.ID].Item2);
            }
        }

        if (parties.Count > 0) {
            SlideViewModel slideParties = SlideViewModel.Bidirectional (slideCurrentIdx, _localisation.Party.Item2, [.. parties]);

            ++ slideCurrentIdx;
            Slides.Add (slideParties);
        }

        return slideCurrentIdx;
    }

    private (IDType, IDType) GenerateSlidesTitles (Simulation simulation, IDType slideCurrentIdx) {
        List<LineViewModel> linesBallots = [];

        foreach (Ballot ballot in simulation.Ballots.Where (b => ! b.IsIncident)) {
            (string title, string name, string[] _, string[] _, string[] _) = _localisation.Ballots[ballot.ID];

            linesBallots.Add (new (title));
            linesBallots.Add (new (StringLineFormatter.Indent (name, 1)));
        }

        SlideViewModel slideBallots = SlideViewModel.Bidirectional (slideCurrentIdx, "Ballots", linesBallots);

        ++ slideCurrentIdx;
        Slides.Add (slideBallots);

        IDType slideTitleIdx = slideCurrentIdx;
        SlideViewModel slideTitle = SlideViewModel.Forward (
            slideCurrentIdx,
            _localisation.Period,
            [_localisation.Date, _localisation.Situation],
            false
        );

        ++ slideCurrentIdx;
        Slides.Add (slideTitle);
        return (slideCurrentIdx, slideTitleIdx);
    }

    private (IDType, Dictionary<IDType, IDType>) GenerateSlidesBallots (Simulation simulation, IDType slideCurrentIdx) {
        IDType ballotIdx = slideCurrentIdx;
        IDType resultBallotIdx = slideCurrentIdx + simulation.Ballots.Count;
        List<SlideViewModel> slidesBallots = [];
        List<SlideViewModel> slidesResultBallots = [];
        Dictionary<IDType, IDType> ballotIDsFinalIdxs = [];
        Dictionary<IDType, IDType> resultBallotIDsFinalIdxs = [];

        // Every Ballot ID is mapped to its index
        foreach (Ballot ballot in simulation.Ballots) {
            (string title, string name, string[] description, string[] _, string[] _) = _localisation.Ballots[ballot.ID];
            SlideViewModel slideBallot = SlideViewModel.Branching (
                slideCurrentIdx,
                title,
                [name, .. description],
                // Pass, fail
                [
                    new (new BallotVoteCondition (true), resultBallotIdx),
                    new (new BallotVoteCondition (false), resultBallotIdx + 1),
                ],
                in _localisation
            );

            ballotIDsFinalIdxs[ballot.ID] = slideCurrentIdx;
            ++ slideCurrentIdx;
            slidesBallots.Add (slideBallot);
            resultBallotIdx += 2; // On the final iteration, this should point to the end results
        }

        IDType resultEndIdx = resultBallotIdx;
        void GenerateSlideResult (Ballot.Result result, string title, string[] results) {
            List<Link<SlideViewModel>> links = [];
            List<LineViewModel> effects = [];

            foreach (Link<Ballot> l in result.Links) {
                if (l.TargetID == Ballot.END) {
                    links.Add (new (l.Condition, resultEndIdx));
                } else {
                    links.Add (new (l.Condition, ballotIDsFinalIdxs[l.TargetID]));
                }
            }

            if (links.Count == 0) {
                links = [new (new AlwaysCondition (), resultEndIdx)];
            }

            foreach (Ballot.Effect e in result.Effects) {
                List<string> effect = [.. e.ToString (simulation, in _localisation).Split ('\n')];

                effects.AddRange (effect.ConvertAll (l => new LineViewModel (l, true)));
            }

            SlideViewModel slideBallotPass = SlideViewModel.Branching (
                slideCurrentIdx,
                title,
                [.. effects, .. results],
                links,
                in _localisation
            );

            ++ slideCurrentIdx; // On the final iteration (fail), this should point to the end results
            slidesResultBallots.Add (slideBallotPass);
        }

        // Result Links are mapped from IDs to indexes
        foreach (Ballot ballot in simulation.Ballots) {
            // Pass, fail
            (string title, string _, string[] _, string[] pass, string[] fail) = _localisation.Ballots[ballot.ID];
            GenerateSlideResult (ballot.Pass, $"{title} Passed", pass);
            GenerateSlideResult (ballot.Fail, $"{title} Failed", fail);
        }

        Dictionary<IDType, IDType> ballotIdxsIds = ballotIDsFinalIdxs.ToDictionary (k => k.Value, k => k.Key);

        Slides.AddRange (slidesBallots);
        Slides.AddRange (slidesResultBallots);
        return (slideCurrentIdx, ballotIdxsIds);
    }

    private IDType GenerateSlidesResults (Simulation simulation, IDType slideCurrentIdx) {
        IDType resultIdxOffset = slideCurrentIdx;
        IDType resultHistoricalIdx = slideCurrentIdx + simulation.Results.Count;
        List<SlideViewModel> slidesResults = [];
        Dictionary<IDType, IDType> resultIDsFinalIdxs = [];

        foreach (Result result in simulation.Results) {
            (string title, string[] description) = _localisation.Results[result.ID];
            List<Link<SlideViewModel>> links = result.Links.ConvertAll (l =>
                new Link<SlideViewModel> (l.Condition, l.TargetID + resultIdxOffset)
            );

            if (links.Count == 0) {
                links = [new (new AlwaysCondition (), resultHistoricalIdx)];
            }

            SlideViewModel slideResult = SlideViewModel.Branching (
                slideCurrentIdx,
                title,
                [.. description],
                links,
                in _localisation
            );

            ++ slideCurrentIdx;
            Slides.Add (slideResult);
        }

        return slideCurrentIdx;
    }

    private void GenerateSlidesEnd (Simulation simulation, IDType slideCurrentIdx) {
        List<string> ballotsPassed = [];
        List<string> ballotsFailed = [];

        foreach (Ballot ballot in simulation.Ballots) {
            string line = StringLineFormatter.Indent (_localisation.Ballots[ballot.ID].Item1, 1);

            if (simulation.History.BallotsProceduresDeclared.TryGetValue (ballot.ID, out SortedSet<IDType>? proceduresDeclared)) {
                var proceduresDeclaredNamesIter = _localisation.Procedures.Where (k => proceduresDeclared.Contains (k.Key))
                    .Select (k => k.Value.Item1);

                string proceduresDeclaredNames = string.Join (", ", proceduresDeclaredNamesIter);

                line += $" ({proceduresDeclaredNames})";
            }

            if (simulation.History.BallotsPassed.Contains (ballot.ID)) {
                ballotsPassed.Add (line);
            } else {
                ballotsFailed.Add (line);
            }
        }

        if (ballotsPassed.Count == 0) {
            ballotsPassed.Add (StringLineFormatter.Indent ("None", 1));
        }

        if (ballotsFailed.Count == 0) {
            ballotsFailed.Add (StringLineFormatter.Indent ("None", 1));
        }

        SlideViewModel slideHistorical = SlideViewModel.Forward (
            slideCurrentIdx,
            "Historical Results",
            ["Passed", .. ballotsPassed, "Failed", .. ballotsFailed]
        );

        ++ slideCurrentIdx;
        Slides.Add (slideHistorical);

        SlideViewModel slideEnd = SlideViewModel.Backward (slideCurrentIdx, "End", [], false);

        Slides.Add (slideEnd);
    }

    private void Simulation_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
        ElectionViewModel election = new (e, in _localisation);

        _election.New (
            election,
            // yeah ok fight me
            () => {
                election!.CompletingElection += _election.Window!.Election_CompletingElectionEventHandler;
                election!.CloseWindow = _election.Window!.Close;
            }
        );
        election.RunElection ();
        _election.ShowDialog ();
        e.PeopleRolesNew = election.PeopleRolesNew;
        e.PeopleFactionsNew = election.PeopleFactionsNew;
    }

    private void Simulation_ReplacingPartyEventHandler (IDType e1, IDType e2) {
        void UpdateSlideResult (IDType slideIdx, Ballot.Result result, string[] results) {
            List<LineViewModel> effects = [];

            foreach (Ballot.Effect e in result.Effects) {
                List<string> effect = [.. e.ToString (_raw, in _localisation).Split ('\n')];

                effects.AddRange (effect.ConvertAll (l => new LineViewModel (l, true)));
            }

            Slides[slideIdx].Description = [.. effects, .. results];
        }

        _localisation.ReplaceParty (e1, e2);
        _context.ReplaceParty (in _localisation);

        foreach (var kv in _ballotIdxsIds.Where (kv => kv.Key > _slideCurrentIdx)) {
            Ballot ballot = _simulation.Ballots[kv.Value];
            (string _, string _, string[] _, string[] pass, string[] fail) = _localisation.Ballots[kv.Value];
            IDType passIdx = Slides[kv.Key].Links[0].Link.TargetID;
            IDType failIdx = Slides[kv.Key].Links[1].Link.TargetID;

            UpdateSlideResult (passIdx, ballot.Pass, pass);
            UpdateSlideResult (failIdx, ballot.Fail, fail);
        }
    }

    public void Context_VotingEventHandler (VotingEventArgs e) {
        if (e.IsPass is bool isPass) {
            if (isPass) {
                _simulation.Context.VotesPass.Add (e.PersonID);
            } else {
                _simulation.Context.VotesPass.Remove (e.PersonID);
            }
        } else if (e.IsFail is bool isFail) {
            if (isFail) {
                _simulation.Context.VotesFail.Add (e.PersonID);
            } else {
                _simulation.Context.VotesFail.Remove (e.PersonID);
            }
        } else if (e.IsAbstain is bool isAbstain) {
            if (isAbstain) {
                _simulation.Context.VotesPass.Remove (e.PersonID);
                _simulation.Context.VotesFail.Remove (e.PersonID);
            }
        } else {
            throw new NotSupportedException ();
        }

        e.VotesPass = _simulation.Context.CalculateVotesPass ();
        e.VotesFail = _simulation.Context.CalculateVotesFail ();
        e.VotesAbstain = _simulation.Context.CalculateVotesAbstain ();
    }

    private void Context_DeclaringProcedureEventHandler (IDType e) {
        DeclareViewModel declare = new (e, _simulation, in _localisation);
        _declare.New (declare);

        declare.ConfirmingProcedure += Declare_ConfirmingProcedureEventHandler;
        _declare.ShowDialog ();
    }

    private void Declare_ConfirmingProcedureEventHandler (ConfirmingProcedureEventArgs e) {
        if (e.IsManual) {
            e.IsManual = false;
            e.IsConfirmed = true;
            e.Message = "Success";
            _simulation.DeclareProcedure (e.PersonID, e.ProcedureID);
        } else {
            SimulationContext.ConfirmationResult result = _simulation.TryConfirmProcedure (e.PersonID, e.ProcedureID);

            switch (result.Type) {
                case Confirmation.ConfirmationType.Always:
                    e.Message = "Success";
                    break;
                // case Confirmation.CostType.DivisionChamber:
                case Confirmation.ConfirmationType.CurrencyValue: {
                    if (result.IsConfirmed is true) {
                        (IDType currencyId, sbyte _) = result.Currency ?? default;

                        e.Message = $"Success: Spent {result.Value} {_localisation.Currencies[currencyId]}";
                    } else {
                        throw new UnreachableException ();
                    }

                    break;
                }
                case Confirmation.ConfirmationType.DiceValue:
                    if (result.IsConfirmed is true) {
                        e.Message = $"Success: Rolled {result.DiceDeclarer}";
                    } else {
                        e.Message = $"Failure: Rolled {result.DiceDeclarer}, but needed at least {result.Value}";
                    }

                    break;
                case Confirmation.ConfirmationType.DiceCurrency: {
                    (IDType currencyId, sbyte currency) = result.Currency ?? default;

                    if (result.IsConfirmed is true) {
                        e.Message = $"Success: Rolled and spent {result.DiceDeclarer} {_localisation.Currencies[currencyId]}";
                    } else {
                        e.Message = $"Failure: Rolled {result.DiceDeclarer}, but only had {currency} {_localisation.Currencies[currencyId]}";
                    }

                    break;
                }
                case Confirmation.ConfirmationType.DiceAdversarial: {
                    if (result.IsConfirmed is true) {
                        if (result.Currency is (IDType currencyId, sbyte _)) {
                            e.Message = $"Success: Rolled and spent {result.DiceDeclarer} {_localisation.Currencies[currencyId]}, while defender rolled {result.DiceDefender}";
                        } else {
                            e.Message = $"Success: Rolled {result.DiceDeclarer}, while defender rolled {result.DiceDefender}";
                        }
                    } else {
                        if (result.Currency is (IDType currencyId, sbyte currency)) {
                            if (result.DiceDefender is null) {
                                e.Message = $"Failure: Rolled {result.DiceDeclarer}, but only had {currency} {_localisation.Currencies[currencyId]}";
                            } else {
                                e.Message = $"Failure: Rolled and spent {result.DiceDeclarer} {_localisation.Currencies[currencyId]}, but defender rolled {result.DiceDefender}";
                            }
                        } else {
                            if (result.DiceDefender is null) {
                                throw new UnreachableException ();
                            } else {
                                e.Message = $"Failure: Rolled {result.DiceDeclarer}, but defender rolled {result.DiceDefender}";
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
                        Link<SlideViewModel> link;

                        if (isPass) {
                            IDType slideIdx = _slide.Links[0].Link.TargetID;
                            link = new (new BallotAlwaysCondition (), slideIdx);
                        } else {
                            IDType slideIdx = _slide.Links[1].Link.TargetID;
                            link = new (new BallotNeverCondition (), slideIdx);
                        }

                        SwitchSlideCommand.Execute (link);
                    }
                }
            } else {
                e.IsManual = true;
            }
        }
    }

    public void InitialisePeople (List<Person> people) {
        _simulation.InitialisePeople (people);
        _context.InitialisePeople (people);
    }

    public RelayCommand<Link<SlideViewModel>> SwitchSlideCommand => new (
        l => {
            if (l.Resolve (_simulation) is IDType slideIdx) {
                if (l.Condition.YieldBallotVote () is bool isPass) {
                    _simulation.EndBallot (isPass);
                }

                SlideCurrentIdx = slideIdx;
            } else {
                throw new UnreachableException ();
            }
        },
        l => l.Condition.Evaluate (_simulation)
    );

    public RelayCommand<string> TrySwitchSlideCommand => new (k => {
        Key key = k switch {
            "L" => Key.Left,
            "U" => Key.Up,
            "D" => Key.Down,
            "R" => Key.Right,
            _ => throw new NotSupportedException (),
        };

        if (_slide.Links.Find (l => l.Key == key) is LinkViewModel l) {
            if (SwitchSlideCommand.CanExecute (l.Link)) {
                SwitchSlideCommand.Execute (l.Link);
            }
        }
    });
}
