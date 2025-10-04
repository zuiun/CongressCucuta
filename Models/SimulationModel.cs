using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal class SimulationModel (ref readonly Simulation simulation) {
    private readonly SimulationContext _context = new ();
    public string State = "State"; // TODO: Initialise
    public SimulationContext Context => _context;
    public List<SlideModel> Slides { get; } = GenerateSlides (in simulation);
    public IDType SlideCurrentIdx { get; set; } = 0;

    private SlideModel GenerateSlideProcedure (ref readonly Simulation simulation, Procedure procedure) {
        List<LineModel> description = [new (procedure.Name, true, procedure.Description)];

        throw new NotImplementedException ();
    }

    private static List<SlideModel> GenerateSlides (ref readonly Simulation simulation) {
        List<SlideModel> slides = [];

#region Introduction
        // TODO: the vast majority of these should really be in special constructors
        IDType slideCurrentIdx = 0;
        SlideLinearModel slideIntro = new (slideCurrentIdx, simulation.Localisation.State, [simulation.Localisation.Government], false);

        ++ slideCurrentIdx;
        slides.Add (slideIntro);

        SlideLinearModel slideContext = new (slideCurrentIdx, "Context", [.. simulation.Localisation.Context]);

        ++ slideCurrentIdx;
        slides.Add (slideContext);

        SlideLinearModel slideTitle = new (
            slideCurrentIdx,
            simulation.Localisation.Period,
            [simulation.Localisation.Date, simulation.Localisation.Situation],
            false
        );

        ++ slideCurrentIdx;
        slides.Add (slideTitle);
#endregion

#region Procedures
        for (byte i = 0; i < simulation.ProceduresGovernmental.Count; ++ i) {
            string title = i > 0 ? "Governmental Procedures (cont.)" : "Governmental Procedures";

            // SlideLinearModel slideProcedure = new (
            //    slideCurrentIdx,
            //    title,

            // );
        }

        // TODO: gov procedures
        // TODO: spec procedures
        // TODO: dec procedures
#endregion

        SlideLinearModel slideTitles = new (slideCurrentIdx, "Titles", [simulation.Localisation.MemberSingular, simulation.Localisation.Speaker]);

        ++ slideCurrentIdx;
        slides.Add (slideTitles);

#region Factions
        List<string> regions = [];

        foreach (Region region in simulation.Regions) {
            if (region.IsActiveStart) {
                regions.Add (region.Name);
                regions.AddRange (region.Description);
            }
        }

        if (regions.Count > 0) {
            SlideLinearModel slideRegions = new (slideCurrentIdx, simulation.Localisation.RegionPlural, [.. regions]);

            ++ slideCurrentIdx;
            slides.Add (slideRegions);
        }

        List<string> parties = [];

        foreach (var party in simulation.Parties) {
            if (party.IsActiveStart) {
                regions.Add (party.Name);
                regions.AddRange (party.Description);
            }
        }

        if (parties.Count > 0) {
            SlideLinearModel slideParties = new (slideCurrentIdx, simulation.Localisation.PartyPlural, [.. parties]);

            ++ slideCurrentIdx;
            slides.Add (slideParties);
        }
#endregion

#region Ballots
        // TODO: compile ballots into one slide
        // TODO: ballots
        IDType ballotIdx = slideCurrentIdx;
        IDType resultBallotIdxFirst = slideCurrentIdx + simulation.Ballots.Count;
        List<SlideModel> slidesBallots = [];
        List<SlideModel> slidesResultBallots = [];
        Dictionary<IDType, IDType> ballotIDsFinalIdxs = [];
        Dictionary<IDType, IDType> resultBallotIDsFinalIdxs = [];

        // First, every Ballot is added and their IDs are mapped to indexes
        foreach (Ballot ballot in simulation.Ballots) {
            string[] description = [ballot.Name, .. ballot.Description];
            SlideBranchingModel slideBallot = new (
                slideCurrentIdx,
                ballot.Title,
                [.. description],
                // Fail (left, first), pass (right, second)
                [
                    new (new BallotVoteCondition (false), resultBallotIdxFirst),
                    new (new BallotVoteCondition (true), resultBallotIdxFirst + 1)
                ]
            );

            ballotIDsFinalIdxs[ballot.ID] = slideCurrentIdx;
            ++ slideCurrentIdx;
            slidesBallots.Add (slideBallot);
            resultBallotIdxFirst += 2; // On the final iteration, this should point to the end results
        }

        IDType resultEndIdx = resultBallotIdxFirst;

        // Then, Result Links can be corectly mapped from IDs to indexes
        foreach (Ballot ballot in simulation.Ballots) {
            // Fail (left, first), pass (right, second)
            List<Link<SlideModel>> linksFail = ballot.FailResult.Links.ConvertAll (l =>
                new Link<SlideModel> (l.Condition, ballotIDsFinalIdxs[l.TargetID])
            );

            if (linksFail.Count == 0) {
                linksFail = [new (new AlwaysCondition (), resultEndIdx)];
            }

            SlideBranchingModel slideBallotFail = new (
                slideCurrentIdx,
                $"{ballot.Title} Failed",
                [.. ballot.FailResult.Description],
                linksFail
            );

            ++ slideCurrentIdx;
            slidesResultBallots.Add (slideBallotFail);

            List<Link<SlideModel>> linksPass = ballot.PassResult.Links.ConvertAll (l =>
                new Link<SlideModel> (l.Condition, ballotIDsFinalIdxs[l.TargetID])
            );

            if (linksPass.Count == 0) {
                linksPass = [new (new AlwaysCondition (), resultEndIdx)];
            }

            SlideBranchingModel slideBallotPass = new (
                slideCurrentIdx,
                $"{ballot.Title} Passed",
                [.. ballot.PassResult.Description],
                linksPass
            );

            ++ slideCurrentIdx; // On the final iteration, this should point to the end results
            slidesResultBallots.Add (slideBallotPass);
        }

        slides.AddRange (slidesBallots);
        slides.AddRange (slidesResultBallots);
#endregion

#region Results
// TODO: end results

#endregion

#region Historical Results
        IDType resultHistoricalIdx = slideCurrentIdx;
        List<string> ballotsPassed = [];
        List<string> ballotsFailed = [];

        foreach (Ballot ballot in simulation.Ballots) {
            string line = StringLineFormatter.Indent (ballot.Title, 1);

            if (simulation.History.BallotsProceduresDeclared.TryGetValue (ballot.ID, out SortedSet<IDType>? proceduresDeclared)) {
                var proceduresDeclaredNamesIter = simulation.ProceduresDeclared.Where (p => proceduresDeclared.Contains (p.ID))
                    .Select (p => p.Name);

                string proceduresDeclaredNames = string.Join (", ", proceduresDeclaredNamesIter);

                line += $" ({proceduresDeclaredNames})";
            }

            if (simulation.History.BallotsPassed.Contains (ballot.ID)) {
                ballotsPassed.Add (line);
            } else {
                ballotsFailed.Add (line);
            }
        }

        SlideLinearModel slideHistorical = new (slideCurrentIdx, "Historical Results", ["Passed", .. ballotsPassed, "Failed", .. ballotsFailed]);

        ++ slideCurrentIdx;
        slides.Add (slideHistorical);
#endregion

        SlideConstantModel slideEnd = new (slideCurrentIdx, "The End", []);

        slides.Add (slideEnd);
        return slides;
    }

    public SlideModel YieldSlide () => Slides[SlideCurrentIdx];

    public IDType? ResolveLink (Link<SlideModel> link) => link.Evaluate (in _context) ? link.TargetID : null;

    public bool EvaluateLink (Link<SlideModel> link) => link.Evaluate (in _context);
}
