using congress_cucuta.Data;
using congress_cucuta.Models;
using congress_cucuta.Views;

namespace congress_cucuta.ViewModels;

internal class SimulationViewModel : ViewModel {
    private readonly SimulationModel _simulation;
    private readonly SlideViewModel _slide = new ();
    private readonly ContextViewModel _context;
    private readonly string _state;
    public SlideViewModel Slide => _slide;
    public ContextViewModel Context => _context;
    public string State => _state;

    public SimulationViewModel (Simulation simulation) {
        _simulation = new (simulation);
        _simulation.CompletingElection += Simulation_CompletingElectionEventHandler;
        _state = _simulation.Localisation.State;
        _context = new (in _simulation);
        _context.Voting += _simulation.Context_VotingEventHandler;

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
