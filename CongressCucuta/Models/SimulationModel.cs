using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Procedures;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Models;

internal class CompletingElectionEventArgs (PreparingElectionEventArgs args, ref readonly Localisation localisation) {
    public List<ElectionModel> Elections = args.Elections.ConvertAll (e => new ElectionModel (e));
    public Dictionary<IDType, SortedSet<IDType>> PeopleRoles = args.PeopleRoles;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactions = args.PeopleFactions;
    public HashSet<IDType> PartiesActive = args.PartiesActive;
    public HashSet<IDType> RegionsActive = args.RegionsActive;
    public SortedList<IDType, string> PeopleNames = new (args.People.ToDictionary (k => k.Key, k => k.Value.Name));
    public Localisation Localisation = localisation;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRolesNew = [];
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactionsNew = [];
}

internal class StartingBallotEventArgs (
    byte votesPassThreshold,
    byte votesFailThreshold,
    byte votesPass,
    byte votesFail,
    byte votesAbstain
) {
    public byte VotesPassThreshold = votesPassThreshold;
    public byte VotesFailThreshold = votesFailThreshold;
    public byte VotesPass = votesPass;
    public byte VotesFail = votesFail;
    public byte VotesAbstain = votesAbstain;
}

internal class SimulationModel {
    private readonly SimulationContext _context;
    private readonly Localisation _localisation;
    private IDType _slideCurrentIdx = 0;
    private readonly IDType _slideTitleIdx;
    private readonly Dictionary<IDType, IDType> _ballotIdxsIds;
    private bool _isBallot = false;
    public SimulationContext Context => _context;
    public Localisation Localisation => _localisation;
    public List<SlideModel> Slides { get; } = [];
    public IDType SlideCurrentIdx {
        get => _slideCurrentIdx;
        set {
            _slideCurrentIdx = value;

            if (_slideCurrentIdx == _slideTitleIdx) {
                _context.StartSetup ();
            } else if (_ballotIdxsIds.TryGetValue (_slideCurrentIdx, out IDType ballotId)) {
                _context.BallotCurrentID = ballotId;
                _isBallot = true;
                _context.IsBallot = true;

                StartingBallotEventArgs args = new (
                    _context.Context.CalculateVotesPassThreshold (),
                    _context.Context.CalculateVotesFailThreshold (),
                    _context.Context.CalculateVotesPass (),
                    _context.Context.CalculateVotesFail (),
                    _context.Context.CalculateVotesAbstain ()
                );

                StartingBallot?.Invoke (args);
            } else if (_isBallot) {
                _isBallot = false;
                _context.IsBallot = false;
                EndingBallot?.Invoke ();
            }
        }
    }
    public event Action<CompletingElectionEventArgs>? CompletingElection;
    public event Action<StartingBallotEventArgs>? StartingBallot;
    public event Action? EndingBallot;

    public SimulationModel (Simulation simulation) {
        _context = new (simulation);
        _context.PreparingElection += Context_PreparingElectionEventHandler;
        _localisation = simulation.Localisation;

        IDType slideCurrentIdx = GenerateSlidesIntroduction ();

        slideCurrentIdx = GenerateSlidesProcedures (simulation, slideCurrentIdx);
        slideCurrentIdx = GenerateSlideRoles (simulation, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesFactions (simulation, slideCurrentIdx);
        (slideCurrentIdx, _slideTitleIdx) = GenerateSlidesTitles (simulation, slideCurrentIdx);
        (slideCurrentIdx, _ballotIdxsIds) = GenerateSlidesBallots (simulation, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesResults (simulation, slideCurrentIdx);
        GenerateSlidesEnd (simulation, slideCurrentIdx);
    }

    private IDType GenerateSlidesIntroduction () {
        IDType slideCurrentIdx = 0;
        SlideForwardModel slideIntro = new (slideCurrentIdx, _localisation.State, [_localisation.Government], false);

        ++ slideCurrentIdx;
        Slides.Add (slideIntro);

        SlideBidirectionalModel slideContext = new (slideCurrentIdx, "Context", [.. _localisation.Context]);

        ++ slideCurrentIdx;
        Slides.Add (slideContext);
        return slideCurrentIdx;
    }

    private IDType GenerateSlidesProcedures (Simulation simulation, IDType slideCurrentIdx ) {
        void GenerateProcedureSlide (string title, Procedure procedure) {
            string procedureFull = procedure.ToString (simulation, in _localisation);
            string[] procedureSplit = procedureFull.Split ('\n');
            LineModel line = new (procedureSplit[0], description: _localisation.Procedures[procedure.ID].Item2);
            SlideBidirectionalModel slideProcedure = new (
                slideCurrentIdx,
                title,
                [line, .. procedureSplit[1 ..]]
            );

            ++ slideCurrentIdx;
            Slides.Add (slideProcedure);
        }

        for (byte i = 0; i < simulation.ProceduresGovernmental.Count; ++ i) {
            string title = i > 0 ? "Governmental Procedures (cont.)" : "Governmental Procedures";
            Procedure procedure = simulation.ProceduresGovernmental[i];

            GenerateProcedureSlide (title, procedure);
        }

        for (byte i = 0; i < simulation.ProceduresSpecial.Count; ++i) {
            string title = i > 0 ? "Special Procedures (cont.)" : "Special Procedures";
            Procedure procedure = simulation.ProceduresSpecial[i];

            if (procedure.IsActiveStart) {
                GenerateProcedureSlide (title, procedure);
            }
        }

        for (byte i = 0; i < simulation.ProceduresDeclared.Count; ++i) {
            string title = i > 0 ? "Declared Procedures (cont.)" : "Declared Procedures";
            Procedure procedure = simulation.ProceduresDeclared[i];

            GenerateProcedureSlide (title, procedure);
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

        SlideBidirectionalModel slideRoles = new (
            slideCurrentIdx,
            "Roles",
            [.. roles]
        );

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
            SlideBidirectionalModel slideRegions = new (slideCurrentIdx, _localisation.Region.Item2, [.. regions]);

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
            SlideBidirectionalModel slideParties = new (slideCurrentIdx, _localisation.Party.Item2, [.. parties]);

            ++ slideCurrentIdx;
            Slides.Add (slideParties);
        }

        return slideCurrentIdx;
    }

    private (IDType, IDType) GenerateSlidesTitles (Simulation simulation, IDType slideCurrentIdx) {
        List<LineModel> linesBallots = [];

        foreach (Ballot ballot in simulation.Ballots.Where (b => !b.IsIncident)) {
            (string title, string name, string[] _, string[] _, string[] _) = _localisation.Ballots[ballot.ID];

            linesBallots.Add (new (title));
            linesBallots.Add (new (StringLineFormatter.Indent (name, 1)));
        }

        SlideBidirectionalModel slideBallots = new (slideCurrentIdx, "Ballots", linesBallots);

        ++ slideCurrentIdx;
        Slides.Add (slideBallots);

        IDType slideTitleIdx = slideCurrentIdx;
        SlideForwardModel slideTitle = new (
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
        List<SlideModel> slidesBallots = [];
        List<SlideModel> slidesResultBallots = [];
        Dictionary<IDType, IDType> ballotIDsFinalIdxs = [];
        Dictionary<IDType, IDType> resultBallotIDsFinalIdxs = [];

        // Every Ballot ID is mapped to its index
        foreach (Ballot ballot in simulation.Ballots) {
            (string title, string name, string[] description, string[] _, string[] _) = _localisation.Ballots[ballot.ID];
            SlideBranchingModel slideBallot = new (
                slideCurrentIdx,
                title,
                [name, .. description],
                // Pass, fail
                [
                    new (new BallotVoteCondition (true), resultBallotIdx),
                    new (new BallotVoteCondition (false), resultBallotIdx + 1),
                ]
            );

            ballotIDsFinalIdxs[ballot.ID] = slideCurrentIdx;
            ++ slideCurrentIdx;
            slidesBallots.Add (slideBallot);
            resultBallotIdx += 2; // On the final iteration, this should point to the end results
        }

        IDType resultEndIdx = resultBallotIdx;

        // Result Links are mapped from IDs to indexes
        foreach (Ballot ballot in simulation.Ballots) {
            // Pass, fail
            (string title, string _, string[] _, string[] pass, string[] fail) = _localisation.Ballots[ballot.ID];
            List<Link<SlideModel>> linksPass = [];
            List<LineModel> effectsPass = [];

            foreach (Link<Ballot> l in ballot.Pass.Links) {
                if (l.TargetID == Ballot.END) {
                    linksPass.Add (new (l.Condition, resultEndIdx));
                } else {
                    linksPass.Add (new (l.Condition, ballotIDsFinalIdxs[l.TargetID]));
                }
            }

            if (linksPass.Count == 0) {
                linksPass = [new (new AlwaysCondition (), resultEndIdx)];
            }

            foreach (Ballot.Effect e in ballot.Pass.Effects) {
                List<string> effect = [.. e.ToString (simulation, in _localisation).Split ('\n')];

                effectsPass.AddRange (effect.ConvertAll (l => new LineModel (l, true)));
            }

            SlideBranchingModel slideBallotPass = new (
                slideCurrentIdx,
                $"{title} Passed",
                [.. effectsPass, .. pass],
                linksPass
            );

            ++ slideCurrentIdx;
            slidesResultBallots.Add (slideBallotPass);

            List<Link<SlideModel>> linksFail = [];
            List<LineModel> effectsFail = [];

            foreach (Link<Ballot> l in ballot.Fail.Links) {
                if (l.TargetID == Ballot.END) {
                    linksFail.Add (new (l.Condition, resultEndIdx));
                } else {
                    linksFail.Add (new (l.Condition, ballotIDsFinalIdxs[l.TargetID]));
                }
            }

            if (linksFail.Count == 0) {
                linksFail = [new (new AlwaysCondition (), resultEndIdx)];
            }

            foreach (Ballot.Effect e in ballot.Fail.Effects) {
                List<string> effect = [.. e.ToString (simulation, in _localisation).Split ('\n')];

                effectsFail.AddRange (effect.ConvertAll (l => new LineModel (l, true)));
            }

            SlideBranchingModel slideBallotFail = new (
                slideCurrentIdx,
                $"{title} Failed",
                [.. effectsFail, .. fail],
                linksFail
            );

            ++ slideCurrentIdx; // On the final iteration, this should point to the end results
            slidesResultBallots.Add (slideBallotFail);
        }

        Dictionary<IDType, IDType> ballotIdxsIds = ballotIDsFinalIdxs.ToDictionary (k => k.Value, k => k.Key);

        Slides.AddRange (slidesBallots);
        Slides.AddRange (slidesResultBallots);
        return (slideCurrentIdx, ballotIdxsIds);
    }

    private IDType GenerateSlidesResults (Simulation simulation, IDType slideCurrentIdx) {
        IDType resultIdxOffset = slideCurrentIdx;
        IDType resultHistoricalIdx = slideCurrentIdx + simulation.Results.Count;
        List<SlideModel> slidesResults = [];
        Dictionary<IDType, IDType> resultIDsFinalIdxs = [];

        foreach (Result result in simulation.Results) {
            (string title, string[] description) = _localisation.Results[result.ID];
            List<Link<SlideModel>> links = result.Links.ConvertAll (l =>
                new Link<SlideModel> (l.Condition, l.TargetID + resultIdxOffset)
            );

            if (links.Count == 0) {
                links = [new (new AlwaysCondition (), resultHistoricalIdx)];
            }

            SlideBranchingModel slideResult = new (
                slideCurrentIdx,
                title,
                [.. description],
                links
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

        SlideForwardModel slideHistorical = new (slideCurrentIdx, "Historical Results", ["Passed", .. ballotsPassed, "Failed", .. ballotsFailed]);

        ++ slideCurrentIdx;
        Slides.Add (slideHistorical);

        SlideBackwardModel slideEnd = new (slideCurrentIdx, "End", []);

        Slides.Add (slideEnd);
    }

    private void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
        CompletingElectionEventArgs args = new (e, in _localisation);

        CompletingElection?.Invoke (args);
        e.PeopleRolesNew = args.PeopleRolesNew;
        e.PeopleFactionsNew = args.PeopleFactionsNew;
    }

    public void Context_VotingEventHandler (VotingEventArgs e) {
        if (e.IsPass is bool isPass) {
            (e.VotesPass, e.VotesFail, e.VotesAbstain) = _context.VotePass (e.PersonID, isPass);
        } else if (e.IsFail is bool isFail) {
            (e.VotesPass, e.VotesFail, e.VotesAbstain) = _context.VoteFail (e.PersonID, isFail);
        } else if (e.IsAbstain is bool isAbstain) {
            (e.VotesPass, e.VotesFail, e.VotesAbstain) = _context.VoteAbstain (e.PersonID, isAbstain);
        } else {
            throw new NotSupportedException ();
        }
    }
}
