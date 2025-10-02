using System.Diagnostics;
using congress_cucuta.Data;
using System.Text;

namespace congress_cucuta.Models;

internal class SimulationModel {
    // End state of ballots
    private readonly IDType _resultBallotIdx;
    // End state of results
    private readonly IDType _resultHistoricalIdx;
    private readonly SimulationContext _context = new ();
    public string State = "State"; // TODO: Initialise
    public SimulationContext Context => _context;
    public List<SlideModel> Slides { get; }
    public IDType SlideCurrentIdx { get; set; } = 0;

    public SimulationModel (ref readonly Simulation simulation) {
        (Slides, _resultBallotIdx, _resultHistoricalIdx) = GenerateSlides (in simulation);

        throw new NotImplementedException ();
    }

    // TODO: THIS IS ONLY FOR TESTING PURPOSES!
    public SimulationModel () {
        State = "Great Para";

        List<SlideModel> slides = [];

        slides.Add (new SlideLinearModel (0, "Slide 1", ["make sure there is Next!", "this is content"]));
        slides.Add (new SlideLinearModel (1, "Slide 2", ["i should be a subtitle"]));
        slides.Add (new SlideConstantModel (2, "Slide 3", ["we shouldn't have any buttons!\nwhat if i'm multiline?"]));
        slides.Add (new SlideLinearModel (3, "Slide 4", ["you should never reach me!"]));
        Slides = slides;
    }

    private static string Indent (string line, byte indentLevel) {
        StringBuilder result = new ();

        for (byte i = 0; i < indentLevel; ++ i) {
            result.Append (LineModel.INDENT);
        }

        result.Append (LineModel.DELIMITER);
        result.Append (line);
        return result.ToString ();
    }

    private static string ToString (Procedure.TargetType? target, string name, params string[] items) {
        return target switch {
            Procedure.TargetType.Every => $"Every {name}:",
            Procedure.TargetType.Except => $"Every {name} except {string.Join (", ", items)}:",
            Procedure.TargetType.Only => $"{string.Join (", ", items)}:",
            _ => throw new NotSupportedException (),
        };
    }

    private SlideModel GenerateSlideProcedure (ref readonly Simulation simulation, Procedure procedure) {
        List<LineModel> description = [new (procedure.Name, true, procedure.Description)];

        foreach (Procedure.Effect effect in procedure.Effects) {
            switch (effect.Action) {
                case Procedure.Effect.ActionType.VotePassAdd: {
                    string[] ballotsIter = [.. effect.FilterIDs.Select (t => _context.Ballots[t].Title)];
                    string everyBallot = Indent (ToString (effect.Filter, "Ballot", ballotsIter), 1);
                    description.Add (everyBallot);
                    string addPassVote = Indent ($"Gains {effect.Value} vote(s) in favour", 2);
                    description.Add (addPassVote);
                    break;
                }
                case Procedure.Effect.ActionType.VoteFailAdd: {
                    string[] ballotsIter = [.. effect.FilterIDs.Select (t => _context.Ballots[t].Title)];
                    string everyBallot = Indent (ToString (effect.Filter, "Ballot", ballotsIter), 1);
                    description.Add (everyBallot);
                    string addPassVote = Indent ($"Gains {effect.Value} vote(s) in opposition", 2);
                    description.Add (addPassVote);
                    break;
                }
                case Procedure.Effect.ActionType.VotePassTwoThirds: {
                    string[] ballotsIter = [.. effect.FilterIDs.Select (t => _context.Ballots[t].Title)];
                    string everyBallot = Indent (ToString (effect.Filter, "Ballot", ballotsIter), 1);
                    description.Add (everyBallot);
                    string addPassVote = Indent ("Needs a two-thirds majority to pass", 2);
                    description.Add (addPassVote);
                    break;
                }
                case Procedure.Effect.ActionType.CurrencyAdd: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.CurrencySubtract: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.CurrencySet: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.ProcedureActivate: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.ElectionRandom: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.ElectionNominated: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.Commitment: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.VotersLimit: {
                    throw new NotImplementedException ();
                }
                case Procedure.Effect.ActionType.Appointment: {
                    throw new NotImplementedException ();
                }
                default:
                    throw new UnreachableException ();
            }
        }

        return new SlideLinearModel (0, "Title", description);
    }

    private static (List<SlideModel>, byte, byte) GenerateSlides (ref readonly Simulation simulation) {
        List<SlideModel> slides = [];

        // TODO: the vast majority of these should really be in special constructors
        IDType slideCurrentIdx = 0;
        SlideLinearModel slideIntro = new (slideCurrentIdx, simulation.History.State, [simulation.History.Government]);
        ++ slideCurrentIdx;
        slides.Add (slideIntro);
        SlideLinearModel slideContext = new (slideCurrentIdx, "Context", [.. simulation.History.Context]);
        ++ slideCurrentIdx;
        slides.Add (slideContext);
        SlideLinearModel slideTitle = new (
            slideCurrentIdx,
            simulation.History.Period,
            [$"{simulation.History.Date}\n{simulation.History.Situation}"]
        );
        ++ slideCurrentIdx;
        slides.Add (slideTitle);

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

        SlideLinearModel slideTitles = new (slideCurrentIdx, "Titles", [simulation.History.Member, simulation.History.Speaker]);
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

        SlideLinearModel slideRegions = new (slideCurrentIdx, simulation.RegionNamePlural, [.. regions]);
        ++ slideCurrentIdx;
        slides.Add (slideRegions);

        List<string> parties = [];

        foreach (var party in simulation.Parties) {
            if (party.IsActiveStart) {
                regions.Add (party.Name);
                regions.AddRange (party.Description);
            }
        }

        SlideLinearModel slideParties = new (slideCurrentIdx, simulation.PartyNamePlural, [.. parties]);
        ++ slideCurrentIdx;
        slides.Add (slideParties);
        #endregion

        // TODO: compile ballots into one slide
        // TODO: ballots
        // TODO: ballot results
        IDType resultBallotIdx = slideCurrentIdx;

        #region Historical Results
        IDType resultHistoricalIdx = slideCurrentIdx;
        List<string> ballotsPassed = [];
        List<string> ballotsFailed = [];

        foreach (Ballot ballot in simulation.Ballots) {
            string line = LineModel.INDENT + LineModel.DELIMITER + ballot.Name;

            if (simulation.History.Path.BallotsProceduresDeclared.TryGetValue (ballot.ID, out SortedSet<IDType>? proceduresDeclared)) {
                var proceduresDeclaredNamesIter = simulation.ProceduresDeclared.Where (p => proceduresDeclared.Contains (p.ID))
                    .Select (p => p.Name);

                string proceduresDeclaredNames = string.Join (", ", proceduresDeclaredNamesIter);

                line += $" ({proceduresDeclaredNames})";
            }

            if (simulation.History.Path.BallotsPassed.Contains (ballot.ID)) {
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
        return (slides, resultBallotIdx, resultHistoricalIdx);
    }

    public SlideModel YieldSlide () => Slides[SlideCurrentIdx];

    public IDType? ResolveLink (Link<SlideModel> link) {
        bool? result = link.Evaluate (in _context);

        return result switch {
            null => SlideCurrentIdx < _resultBallotIdx ? _resultBallotIdx : _resultHistoricalIdx,
            true => link.TargetID,
            _ => null,
        };
    }

    public bool EvaluateLink (Link<SlideModel> link) => link.Evaluate (in _context) is null or true;
}
