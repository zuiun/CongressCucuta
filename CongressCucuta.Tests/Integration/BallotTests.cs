using CongressCucuta.Core;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;
using CongressCucuta.Views;

namespace CongressCucuta.Tests.Integration;

[TestClass]
public sealed class BallotTests {
    [TestMethod]
    [DataRow (true, (byte) 1)]
    [DataRow (false, (byte) 0)]
    public void Ballot_VotePass_MutatesExpected (bool isPass, byte expected) {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        vm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        vm.SlideCurrentIdx = title;
        vm.SlideCurrentIdx = ballot0;
        vm.Context.People[0].IsPass = ! isPass;

        vm.Context.People[0].IsPass = isPass;

        Assert.AreEqual (expected, vm.Context.VotesPass);
    }

    [TestMethod]
    [DataRow (true, (byte) 1)]
    [DataRow (false, (byte) 0)]
    public void Ballot_VoteFail_MutatesExpected (bool isFail, byte expected) {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        vm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        vm.SlideCurrentIdx = title;
        vm.SlideCurrentIdx = ballot0;
        vm.Context.People[0].IsFail = ! isFail;

        vm.Context.People[0].IsFail = isFail;

        Assert.AreEqual (expected, vm.Context.VotesFail);
    }

    [TestMethod]
    [DataRow (true, (byte) 1)]
    [DataRow (false, (byte) 1)]
    public void Ballot_VoteAbstain_MutatesExpected (bool isAbstain, byte expected) {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        vm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        vm.SlideCurrentIdx = title;
        vm.SlideCurrentIdx = ballot0;
        vm.Context.People[0].IsAbstain = ! isAbstain;

        vm.Context.People[0].IsAbstain = isAbstain;

        Assert.AreEqual (expected, vm.Context.VotesAbstain);
    }

    [TestMethod]
    public void Ballot_VoteResetPeople_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotLimit, [255])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0]);
        SimulationViewModel simulationVm = new (simulation, declare: declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].IsPass = true;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.AreEqual (0, simulationVm.Context.VotesPass);
        Assert.AreEqual (0, simulationVm.Context.VotesFail);
        Assert.AreEqual (1, simulationVm.Context.VotesAbstain);
    }

    [TestMethod]
    public void Ballot_VoteResetFactions_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new (Procedure.Effect.EffectType.ElectionParty, [], 1)]
        );
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotLimit, [255])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0]);
        SimulationViewModel simulationVm = new (simulation, election, declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.FactionsPeople[0].People[0].IsPass = true;
        simulationVm.Context.FactionsPeople[0].People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.AreEqual (0, simulationVm.Context.VotesPass);
        Assert.AreEqual (0, simulationVm.Context.VotesFail);
        Assert.AreEqual (1, simulationVm.Context.VotesAbstain);
    }

    [TestMethod]
    public void Ballot_EndPassTrue_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].IsPass = true;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;
        
        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (simulationVm.Context.Ballots[0].IsPass);
        Assert.AreEqual (1, simulationVm.Context.Ballots[0].VotesPass);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesFail);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesAbstain);
        Assert.AreEqual ("0", simulationVm.Context.Ballots[0].Title);
        Assert.AreEqual ("Ballot 0", simulationVm.Context.Ballots[0].Name);
        Assert.AreEqual ("Procedure 3", simulationVm.Context.Ballots[0].Procedures[0]);
    }

    [TestMethod]
    public void Ballot_EndPassFalse_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].IsPass = false;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (simulationVm.Context.Ballots[0].IsPass);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesPass);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesFail);
        Assert.AreEqual (1, simulationVm.Context.Ballots[0].VotesAbstain);
        Assert.AreEqual ("0", simulationVm.Context.Ballots[0].Title);
        Assert.AreEqual ("Ballot 0", simulationVm.Context.Ballots[0].Name);
        Assert.AreEqual ("Procedure 3", simulationVm.Context.Ballots[0].Procedures[0]);
    }

    [TestMethod]
    public void Ballot_EndFailTrue_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotFail, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].IsFail = true;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (simulationVm.Context.Ballots[0].IsPass);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesPass);
        Assert.AreEqual (1, simulationVm.Context.Ballots[0].VotesFail);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesAbstain);
        Assert.AreEqual ("0", simulationVm.Context.Ballots[0].Title);
        Assert.AreEqual ("Ballot 0", simulationVm.Context.Ballots[0].Name);
        Assert.AreEqual ("Procedure 3", simulationVm.Context.Ballots[0].Procedures[0]);
    }

    [TestMethod]
    public void Ballot_EndFailFalse_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotFail, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].IsFail = false;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (simulationVm.Context.Ballots[0].IsPass);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesPass);
        Assert.AreEqual (0, simulationVm.Context.Ballots[0].VotesFail);
        Assert.AreEqual (1, simulationVm.Context.Ballots[0].VotesAbstain);
        Assert.AreEqual ("0", simulationVm.Context.Ballots[0].Title);
        Assert.AreEqual ("Ballot 0", simulationVm.Context.Ballots[0].Name);
        Assert.AreEqual ("Procedure 3", simulationVm.Context.Ballots[0].Procedures[0]);
    }
}
