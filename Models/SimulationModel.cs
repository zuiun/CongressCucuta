using congress_cucuta.Converters;
using congress_cucuta.Data;
using System.Windows;
using System.Windows.Controls;

namespace congress_cucuta.Models;

internal class SimulationModel (Simulation simulation) {
    private readonly SimulationContext _context = new ();
    public string State = "State"; // TODO: Initialise
    public SimulationContext Context => _context;
    public Localisation Localisation => simulation.Localisation;
    public List<SlideModel> Slides { get; } = GenerateSlides (in simulation);
    public IDType SlideCurrentIdx { get; set; } = 0;

    private static IDType GenerateSlidesIntroduction (ref readonly Localisation localisation, ref List<SlideModel> slides) {
        IDType slideCurrentIdx = 0;
        SlideForwardModel slideIntro = new (slideCurrentIdx, localisation.State, [localisation.Government], false);

        ++ slideCurrentIdx;
        slides.Add (slideIntro);

        SlideBidirectionalModel slideContext = new (slideCurrentIdx, "Context", [.. localisation.Context]);

        ++ slideCurrentIdx;
        slides.Add (slideContext);

        SlideBidirectionalModel slideTitle = new (
            slideCurrentIdx,
            localisation.Period,
            [localisation.Date, localisation.Situation],
            false
        );

        ++ slideCurrentIdx;
        slides.Add (slideTitle);
        return slideCurrentIdx;
    }

    private static IDType GenerateSlidesProcedures (
        ref readonly Simulation simulation,
        ref readonly Localisation localisation,
        ref List<SlideModel> slides,
        IDType slideCurrentIdx
    ) {
        //for (byte i = 0; i < simulation.ProceduresGovernmental.Count; ++i) {
        //    string title = i > 0 ? "Governmental Procedures (cont.)" : "Governmental Procedures";

        // SlideLinkedModel slideProcedure = new (
        //    slideCurrentIdx,
        //    title,

        // );
        //}

        // TODO: gov procedures
        // TODO: spec procedures
        // TODO: dec procedures
        return slideCurrentIdx;
    }

    private static IDType GenerateSlidesPermissions (
        ref readonly Simulation simulation,
        ref readonly Localisation localisation,
        ref List<SlideModel> slides,
        IDType slideCurrentIdx
    ) {
        Permissions member = simulation.RolesPermissions.Where (k => k.Key.ID == Role.MEMBER)
            .Select (k => k.Value)
            .First ();
        SlideBidirectionalModel slideMember = new (
            slideCurrentIdx,
            localisation.Member.Item1,
            [.. member.ToString ().Split ('\n')]
        );

        ++ slideCurrentIdx;
        slides.Add (slideMember);

        if (localisation.Roles.TryGetValue (Role.HEAD_GOVERNMENT, out (string, string) headGovernmentTitle)) {
            Permissions headGovernment = simulation.RolesPermissions.Where (k => k.Key.ID == Role.HEAD_GOVERNMENT)
                .Select (k => k.Value)
                .First ();
            SlideBidirectionalModel slideHeadGovernment = new (
                slideCurrentIdx,
                headGovernmentTitle.Item1,
                [.. headGovernment.ToString ().Split ('\n')]
            );

            ++ slideCurrentIdx;
            slides.Add (slideHeadGovernment);
        }

        if (localisation.Roles.TryGetValue (Role.HEAD_STATE, out (string, string) headStateTitle)) {
            Permissions headState = simulation.RolesPermissions.Where (k => k.Key.ID == Role.HEAD_STATE)
                .Select (k => k.Value)
                .First ();
            SlideBidirectionalModel slideHeadState = new (
                slideCurrentIdx,
                headStateTitle.Item1,
                [.. headState.ToString ().Split ('\n')]
            );

            ++ slideCurrentIdx;
            slides.Add (slideHeadState);
        }

        if (simulation.Parties.Count > 0) {
            IDType anyId = simulation.Parties[0].ID;

            if (localisation.Roles.TryGetValue (Role.LEADER_PARTY, out (string, string) leaderPartyTitles)) {
                Permissions leaderParty = simulation.RolesPermissions.Where (k => k.Key.ID == Role.LEADER_PARTY)
                    .Select (k => k.Value)
                    .First ();
                SlideBidirectionalModel slideLeaderParty = new (
                    slideCurrentIdx,
                    leaderPartyTitles.Item2,
                    [.. leaderParty.ToString ().Split ('\n')]
                );

                ++ slideCurrentIdx;
                slides.Add (slideLeaderParty);
            } else if (localisation.Roles.Keys.Any (r => r.ID == anyId)) {
                foreach (Faction party in simulation.Parties) {
                    string leaderPartyTitle = localisation.Roles[party.ID].Item1;
                    Permissions leaderParty = simulation.RolesPermissions.Where (k => k.Key.ID == party.ID)
                        .Select (k => k.Value)
                        .First ();
                    SlideBidirectionalModel slideLeaderParty = new (
                        slideCurrentIdx,
                        leaderPartyTitle,
                        [.. leaderParty.ToString ().Split ('\n')]
                    );

                    ++ slideCurrentIdx;
                    slides.Add (slideLeaderParty);
                }
            }
        }
        
        if (simulation.Regions.Count > 0) {
            IDType anyId = simulation.Regions[0].ID;

            if (localisation.Roles.TryGetValue (Role.LEADER_REGION, out (string, string) leaderRegionTitles)) {
                Permissions leaderRegion = simulation.RolesPermissions.Where (k => k.Key.ID == Role.LEADER_REGION)
                    .Select (k => k.Value)
                    .First ();
                SlideBidirectionalModel slideLeaderRegion = new (
                    slideCurrentIdx,
                    leaderRegionTitles.Item2,
                    [.. leaderRegion.ToString ().Split ('\n')]
                );

                ++ slideCurrentIdx;
                slides.Add (slideLeaderRegion);
            } else if (localisation.Roles.Keys.Any (r => r.ID == anyId)) {
                foreach (Faction region in simulation.Regions) {
                    string leaderRegionTitle = localisation.Roles[region.ID].Item1;
                    Permissions leaderRegion = simulation.RolesPermissions.Where (k => k.Key.ID == region.ID)
                        .Select (k => k.Value)
                        .First ();
                    SlideBidirectionalModel slideLeaderRegion = new (
                        slideCurrentIdx,
                        leaderRegionTitle,
                        [.. leaderRegion.ToString ().Split ('\n')]
                    );

                    ++ slideCurrentIdx;
                    slides.Add (slideLeaderRegion);
                }
            }
        }
        
        return slideCurrentIdx;
    }

    private static IDType GenerateSlidesFactions (
        ref readonly Simulation simulation,
        ref readonly Localisation localisation,
        ref List<SlideModel> slides,
        IDType slideCurrentIdx
    ) {
        List<string> regions = [];

        foreach (Faction region in simulation.Regions) {
            if (region.IsActiveStart) {
                (string name, string[] description, string _) = localisation.Regions[region.ID];

                regions.Add (name);
                regions.AddRange (description);
            }
        }

        if (regions.Count > 0) {
            SlideBidirectionalModel slideRegions = new (slideCurrentIdx, localisation.Region.Item2, [.. regions]);

            ++ slideCurrentIdx;
            slides.Add (slideRegions);
        }

        List<string> parties = [];

        foreach (Faction party in simulation.Parties) {
            if (party.IsActiveStart) {
                parties.Add (localisation.GetFactionAndAbbreviation (party.ID));
                parties.AddRange (localisation.Parties[party.ID].Item2);
            }
        }

        if (parties.Count > 0) {
            SlideBidirectionalModel slideParties = new (slideCurrentIdx, localisation.Party.Item2, [.. parties]);

            ++ slideCurrentIdx;
            slides.Add (slideParties);
        }

        return slideCurrentIdx;
    }

    private static IDType GenerateSlidesBallots (
        ref readonly Simulation simulation,
        ref readonly Localisation localisation,
        ref List<SlideModel> slides,
        IDType slideCurrentIdx
    ) {
        List<LineModel> linesBallots = [];

        foreach (Ballot ballot in simulation.Ballots.Where (b => !b.IsIncident)) {
            (string title, string name, string[] _, string[] _, string[] _) = localisation.Ballots[ballot.ID];

            linesBallots.Add (new (title));
            linesBallots.Add (new (StringLineFormatter.Indent (name, 1)));
        }

        SlideBidirectionalModel slideBallots = new (slideCurrentIdx, "Ballots", linesBallots);

        ++ slideCurrentIdx;
        slides.Add (slideBallots);

        IDType ballotIdx = slideCurrentIdx;
        IDType resultBallotIdx = slideCurrentIdx + simulation.Ballots.Count;
        List<SlideModel> slidesBallots = [];
        List<SlideModel> slidesResultBallots = [];
        Dictionary<IDType, IDType> ballotIDsFinalIdxs = [];
        Dictionary<IDType, IDType> resultBallotIDsFinalIdxs = [];

        // First, every Ballot ID is mapped to its index
        foreach (Ballot ballot in simulation.Ballots) {
            (string title, string name, string[] description, string[] _, string[] _) = localisation.Ballots[ballot.ID];
            SlideBranchingModel slideBallot = new (
                slideCurrentIdx,
                title,
                [name, .. description],
                // Fail (left, first), pass (right, second)
                // TODO: Convert Condition back to BallotVoteCondition later
                [
                    new (new AlwaysCondition (), resultBallotIdx),
                    new (new AlwaysCondition (), resultBallotIdx + 1)
                ]
            );

            ballotIDsFinalIdxs[ballot.ID] = slideCurrentIdx;
            ++ slideCurrentIdx;
            slidesBallots.Add (slideBallot);
            resultBallotIdx += 2; // On the final iteration, this should point to the end results
        }

        IDType resultEndIdx = resultBallotIdx;

        // Then, Result Links can be corectly mapped from IDs to indexes
        foreach (Ballot ballot in simulation.Ballots) {
            (string title, string _, string[] _, string[] pass, string[] fail) = localisation.Ballots[ballot.ID];
            // Fail (left, first), pass (right, second)
            List<Link<SlideModel>> linksFail = ballot.FailResult.Links.ConvertAll (l =>
                new Link<SlideModel> (l.Condition, ballotIDsFinalIdxs[l.TargetID])
            );
            List<LineModel> effectsFail = [];

            if (linksFail.Count == 0) {
                linksFail = [new (new AlwaysCondition (), resultEndIdx)];
            }

            foreach (Ballot.Effect e in ballot.FailResult.Effects) {
                List<string> effect = [.. e.ToString (in localisation).Split ('\n')];

                effectsFail = effect.ConvertAll (l => new LineModel (l, true));
            }

            SlideBranchingModel slideBallotFail = new (
                slideCurrentIdx,
                $"{title} Failed",
                [.. effectsFail, .. fail],
                linksFail
            );

            ++ slideCurrentIdx;
            slidesResultBallots.Add (slideBallotFail);

            List<Link<SlideModel>> linksPass = ballot.PassResult.Links.ConvertAll (l =>
                new Link<SlideModel> (l.Condition, ballotIDsFinalIdxs[l.TargetID])
            );
            List<LineModel> effectsPass = [];

            if (linksPass.Count == 0) {
                linksPass = [new (new AlwaysCondition (), resultEndIdx)];
            }

            foreach (Ballot.Effect e in ballot.PassResult.Effects) {
                List<string> effect = [.. e.ToString (in localisation).Split ('\n')];

                effectsPass = effect.ConvertAll (l => new LineModel (l, true));
            }

            SlideBranchingModel slideBallotPass = new (
                slideCurrentIdx,
                $"{title} Passed",
                [.. effectsPass, .. pass],
                linksPass
            );

            ++ slideCurrentIdx; // On the final iteration, this should point to the end results
            slidesResultBallots.Add (slideBallotPass);
        }

        slides.AddRange (slidesBallots);
        slides.AddRange (slidesResultBallots);
        return slideCurrentIdx;
    }

    private static IDType GenerateSlidesResults (
        ref readonly Simulation simulation,
        ref readonly Localisation localisation,
        ref List<SlideModel> slides,
        IDType slideCurrentIdx
    ) {
        IDType resultIdxOffset = slideCurrentIdx;
        IDType resultHistoricalIdx = slideCurrentIdx + simulation.Results.Count;
        List<SlideModel> slidesResults = [];
        Dictionary<IDType, IDType> resultIDsFinalIdxs = [];

        foreach (Result result in simulation.Results) {
            (string title, string[] description) = localisation.Results[result.ID];
            // TODO: Convert Condition back to l.Condition later
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
            slides.Add (slideResult);
        }

        return slideCurrentIdx;
    }

    private static void GenerateSlidesEnd (
        ref readonly Simulation simulation,
        ref readonly Localisation localisation,
        ref List<SlideModel> slides,
        IDType slideCurrentIdx
    ) {
        List<string> ballotsPassed = [];
        List<string> ballotsFailed = [];

        foreach (Ballot ballot in simulation.Ballots) {
            string line = StringLineFormatter.Indent (localisation.Ballots[ballot.ID].Item1, 1);

            if (simulation.History.BallotsProceduresDeclared.TryGetValue (ballot.ID, out SortedSet<IDType>? proceduresDeclared)) {
                var proceduresDeclaredNamesIter = localisation.Procedures.Where (k => proceduresDeclared.Contains (k.Key))
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

        SlideForwardModel slideHistorical = new (slideCurrentIdx, "Historical Results", ["Passed", .. ballotsPassed, "Failed", .. ballotsFailed]);

        ++ slideCurrentIdx;
        slides.Add (slideHistorical);

        SlideBackwardModel slideEnd = new (slideCurrentIdx, "The End", []);

        slides.Add (slideEnd);
    }

    private static List<SlideModel> GenerateSlides (ref readonly Simulation simulation) {
        Localisation localisation = simulation.Localisation;
        List<SlideModel> slides = [];
        IDType slideCurrentIdx;

        slideCurrentIdx = GenerateSlidesIntroduction (in localisation, ref slides);
        slideCurrentIdx = GenerateSlidesProcedures (in simulation, in localisation, ref slides, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesPermissions (in simulation, in localisation, ref slides, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesFactions (in simulation, in localisation, ref slides, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesBallots (in simulation, in localisation, ref slides, slideCurrentIdx);
        slideCurrentIdx = GenerateSlidesResults (in simulation, in localisation, ref slides, slideCurrentIdx);
        GenerateSlidesEnd (in simulation, in localisation, ref slides, slideCurrentIdx);
        return slides;
    }

    public IDType? ResolveLink (Link<SlideModel> link) => link.Evaluate (in _context) ? link.TargetID : null;

    public bool EvaluateLink (Link<SlideModel> link) => link.Evaluate (in _context);
}
