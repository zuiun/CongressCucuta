using congress_cucuta.Data;
using static congress_cucuta.Data.Ballot;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace congress_cucuta.Models;

internal class SimulationModel {
    public List<SlideModel> Slides { get; }
    public byte SlideCurrentIdx { get; } = 0;

    // End state of ballots
    private readonly byte resultBallotIdx;
    // End state of results
    private readonly byte resultHistoricalIdx;
    private readonly SimulationContext context = new ();

    public SimulationModel (Simulation simulation) {
//        Intro slide (state, government type)
//Context slide
//Title slide (date, event, period)
//Governmental procedures
//Special procedures
//Declared procedures
//Titles slide (member, speaker)
//Regions
//Parties
//Ballots list slide
//Ballots
//End results
//Historical path slide
//The End

        throw new NotImplementedException ();
    }
}
