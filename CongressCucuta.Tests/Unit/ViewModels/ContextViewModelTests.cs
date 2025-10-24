using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class ContextViewModelTests {
    [TestMethod]
    public void Constructor_Procedures_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;

        ContextViewModel actual = new (context, simulation, in localisation);

        Assert.AreEqual<IDType> (0, actual.ProceduresGovernmental[0].ID);
        Assert.AreEqual ("Procedure 0", actual.ProceduresGovernmental[0].Name);
        Assert.DoesNotContain ("Procedure 0", actual.ProceduresGovernmental[0].Effects);
        Assert.HasCount (1, actual.ProceduresSpecial);
        Assert.AreEqual<IDType> (1, actual.ProceduresSpecial[0].ID);
        Assert.AreEqual ("Procedure 1", actual.ProceduresSpecial[0].Name);
        Assert.DoesNotContain ("Procedure 1", actual.ProceduresSpecial[0].Effects);
        Assert.AreEqual<IDType> (3, actual.ProceduresDeclared[0].ID);
        Assert.AreEqual ("Procedure 3", actual.ProceduresDeclared[0].Name);
        Assert.DoesNotContain ("Procedure 3", actual.ProceduresDeclared[0].Effects);
    }

    [TestMethod]
    public void InitialisePeople_Normal_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation);

        IDType expected = 0;
        vm.InitialisePeople ([new (expected, "0")]);

        Assert.AreEqual (expected, vm.People[0].ID);
        Assert.AreEqual ("0", vm.People[0].Name);
    }

    [TestMethod]
    public void StartBallot_Votes_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation);

        vm.StartBallot (1, 2, 3, 4, 5);

        Assert.AreEqual (1, vm.VotesPassThreshold);
        Assert.AreEqual (2, vm.VotesFailThreshold);
        Assert.AreEqual (3, vm.VotesPass);
        Assert.AreEqual (4, vm.VotesFail);
        Assert.AreEqual (5, vm.VotesAbstain);
        Assert.IsTrue (vm.IsBallotCount);
    }

    [TestMethod]
    public void StartBallot_Faction_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation) {
            IsPeople = false,
            IsFaction = true,
        };
        vm.FactionsPeople.Add (new (0, "Faction"));
        vm.FactionsPeople[0].People.Add (new (0, "Person", false));

        vm.StartBallot (0, 0, 0, 0, 0);

        Assert.IsTrue (vm.FactionsPeople[0].People[0].IsInteractable);
    }

    [TestMethod]
    public void StartBallot_NoFaction_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation);
        vm.InitialisePeople ([new (0, "0")]);

        vm.StartBallot (0, 0, 0, 0, 0);

        Assert.IsTrue (vm.People[0].IsInteractable);
    }

    [TestMethod]
    public void EndBallot_Votes_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation);

        vm.EndBallot ();

        Assert.AreEqual (0, vm.VotesPassThreshold);
        Assert.AreEqual (0, vm.VotesFailThreshold);
        Assert.AreEqual (0, vm.VotesPass);
        Assert.AreEqual (0, vm.VotesFail);
        Assert.AreEqual (0, vm.VotesAbstain);
        Assert.IsFalse (vm.IsBallotCount);
    }

    [TestMethod]
    public void EndBallot_Faction_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation) {
            IsPeople = false,
            IsFaction = true,
        };
        vm.FactionsPeople.Add (new (0, "Faction"));
        vm.FactionsPeople[0].People.Add (new (0, "Person", false));

        vm.EndBallot ();

        Assert.IsFalse (vm.FactionsPeople[0].People[0].IsInteractable);
    }

    [TestMethod]
    public void EndBallot_NoFaction_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Localisation localisation = simulation.Localisation;
        ContextViewModel vm = new (context, simulation, in localisation);
        vm.InitialisePeople ([new (0, "0")]);

        vm.EndBallot ();

        Assert.IsFalse (vm.People[0].IsInteractable);
    }
}
