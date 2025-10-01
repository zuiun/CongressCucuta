using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal class SimulationModel {
    public const string NEW_LINE = "&#x0a;";

    private byte slideCurrentIdx = 0;
    // End state of ballots
    private readonly byte resultBallotIdx;
    // End state of results
    private readonly byte resultHistoricalIdx;
    private readonly SimulationContext context = new ();

    public SimulationModel (ref readonly Simulation simulation) {
        (List<SlideModel> slides, byte resultBallotIdx, byte resultHistoricalIdx) = GenerateSlides (in simulation);

        Slides = slides;
        this.resultBallotIdx = resultBallotIdx;
        this.resultHistoricalIdx = resultHistoricalIdx;
        throw new NotImplementedException ();
    }
    
    private static (List<SlideModel>, byte, byte) GenerateSlides (ref readonly Simulation simulation) {
        List<SlideModel> slides = [];

        byte slideCurrentIdx = 0;
        SlideLinearModel slideIntro = new (slideCurrentIdx ++, simulation.History.State, [simulation.History.Government]);
        slides.Add (slideIntro);
        SlideLinearModel slideContext = new (slideCurrentIdx ++, "Context", [.. simulation.History.Context]);
        slides.Add (slideContext);
        SlideLinearModel slideTitle = new (
            slideCurrentIdx ++,
            simulation.History.Period,
            [$"{simulation.History.Date}{NEW_LINE}{simulation.History.Situation}"]
        );
        slides.Add (slideTitle);
        // TODO: gov procedures
        // TODO: spec procedures
        // TODO: dec procedures
        SlideLinearModel slideTitles = new (slideCurrentIdx ++, "Titles", [simulation.History.Member, simulation.History.Speaker]);
        slides.Add (slideTitles);

#region Factions
        List<string> regions = [];

        foreach (var region in simulation.Regions) {
            regions.Add (region.Name);
            regions.AddRange (region.Description);
        }

        SlideLinearModel slideRegions = new (slideCurrentIdx ++, simulation.RegionNamePlural, [.. regions]);
        slides.Add (slideRegions);

        List<string> parties = [];

        foreach (var party in simulation.Parties) {
            regions.Add (party.Name);
            regions.AddRange (party.Description);
        }

        SlideLinearModel slideParties = new (slideCurrentIdx ++, simulation.PartyNamePlural, [.. parties]);
        slides.Add (slideParties);
#endregion

        // TODO: compile ballots into one slide
        // TODO: ballots
        // TODO: ballot results
        byte resultBallotIdx = slideCurrentIdx;

#region Historical Results
        byte resultHistoricalIdx = slideCurrentIdx;
        List<string> ballotsPassed = [];
        List<string> ballotsFailed = [];

        foreach (var ballot in simulation.Ballots) {
            string line = $"{LineModel.INDENT}{LineModel.DELIMITER}{ballot.Name}";

            if (simulation.History.Path.BallotsProceduresDeclared.TryGetValue (ballot.ID, out SortedSet<IDType>? proceduresDeclared)) {
                var proceduresDeclaredNamesIter = simulation.ProceduresDeclared.Where (p => proceduresDeclared.Contains (p.ID))
                    .Select (p => p.Name);
                string proceduresDeclaredNames = string.Join (", ", proceduresDeclaredNamesIter);

                line = $"{line} ({proceduresDeclaredNames})";
            }

            if (simulation.History.Path.BallotsPassed.Contains (ballot.ID)) {
                ballotsPassed.Add (line);
            } else {
                ballotsFailed.Add (line);
            }
        }

        SlideLinearModel slideHistorical = new (slideCurrentIdx ++, "Historical Results", ["Passed", .. ballotsPassed, "Failed", .. ballotsFailed]);
        slides.Add (slideHistorical);
#endregion

        SlideConstantModel slideEnd = new (slideCurrentIdx ++, "The End", []);
        slides.Add (slideEnd);

        return (slides, resultBallotIdx, resultHistoricalIdx);
    }

    public SlideModel YieldSlide () => Slides[slideCurrentIdx];

    public SlideModel SwitchSlide () {
        IDType? slideIdx = Slides[slideCurrentIdx].YieldNext (in context);

        if (slideIdx is null) {
            slideCurrentIdx = slideCurrentIdx < resultBallotIdx ? resultBallotIdx : slideCurrentIdx;
        } else {
            slideCurrentIdx = slideIdx;
        }

        return Slides[slideCurrentIdx];
    }

    public List<SlideModel> Slides { get; }
}
