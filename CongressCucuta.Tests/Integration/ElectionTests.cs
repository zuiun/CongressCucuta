using CongressCucuta.Core;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;
using CongressCucuta.Views;

namespace CongressCucuta.Tests.Integration;

[TestClass]
public sealed class ElectionTests {
    [TestMethod]
    public void Election_RunShuffleRemove_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [], 1)]
        );
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.DissolveParty, [2])], []),
            new Ballot.Result ([], [])
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0]);
        SimulationViewModel simulationVm = new (simulation, election, declare, generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.FactionsPeople[0].People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;
        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        ElectionViewModel actual = election.DataContext!;

        Assert.AreEqual ("Dissolution of Party 2 (2)", actual.Title);
        Assert.AreEqual ("Party 3", actual.GroupsPeople[0].Name);
        Assert.AreEqual<IDType> (3, actual.GroupsPeople[0].People[0].FactionID);
        Assert.AreEqual ("0 [Party 3]", actual.GroupsPeople[0].People[0].Name);
        Assert.DoesNotContain (2, actual.PeopleRolesNew[0]);
        Assert.AreEqual (3, actual.PeopleFactionsNew[0].Item1);
        Assert.AreEqual<IDType> (3, simulationVm.Context.FactionsPeople[0].ID);
        Assert.AreEqual ("Party 3", simulationVm.Context.FactionsPeople[0].Name);
        Assert.IsNull (simulationVm.Context.FactionsPeople[0].Description);
        Assert.IsTrue (simulationVm.Context.FactionsPeople[0].IsCurrency);
        Assert.IsFalse (simulationVm.Context.FactionsPeople[0].IsNotCurrency);
        Assert.AreEqual ("Currency 3", simulationVm.Context.FactionsPeople[0].Currency);
        Assert.AreEqual (1, simulationVm.Context.FactionsPeople[0].Value);
    }

    [TestMethod]
    public void Election_RunShuffleAdd_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.Parties[1] = new (3, false);
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [], 1)]
        );
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.FoundParty, [3], 1)], []),
            new Ballot.Result ([], [])
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0]);
        SimulationViewModel simulationVm = new (simulation, election, declare, generator);
        simulationVm.InitialisePeople ([new (0, "0"), new (1, "1")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.FactionsPeople[0].People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;
        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        ElectionViewModel actual = election.DataContext!;

        Assert.AreEqual ("Founding of Party 3", actual.Title);
        Assert.AreEqual ("Party 3", actual.GroupsPeople[0].Name);
        Assert.AreEqual<IDType> (3, actual.GroupsPeople[0].People[0].FactionID);
        Assert.AreEqual ("1 [Party 3]", actual.GroupsPeople[0].People[0].Name);
        Assert.Contains (3, actual.PeopleRolesNew[1]);
        Assert.AreEqual (3, actual.PeopleFactionsNew[1].Item1);
        Assert.AreEqual<IDType> (2, simulationVm.Context.FactionsPeople[0].ID);
        Assert.AreEqual ("2", simulationVm.Context.FactionsPeople[0].Name);
        Assert.AreEqual ("Party 2", simulationVm.Context.FactionsPeople[0].Description);
        Assert.IsTrue (simulationVm.Context.FactionsPeople[0].IsCurrency);
        Assert.IsFalse (simulationVm.Context.FactionsPeople[0].IsNotCurrency);
        Assert.AreEqual ("Currency 2", simulationVm.Context.FactionsPeople[0].Currency);
        Assert.AreEqual (1, simulationVm.Context.FactionsPeople[0].Value);
    }

    [TestMethod]
    public void Election_RunRegion_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.Regions.RemoveAt (1);
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, [], 1)]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        SimulationViewModel simulationVm = new (simulation, election: election);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        simulationVm.SlideCurrentIdx = title;

        ElectionViewModel actual = election.DataContext!;

        Assert.AreEqual ("Procedure 0", actual.Title);
        Assert.AreEqual ("Region 0", actual.GroupsPeople[0].Name);
        Assert.AreEqual<IDType> (0, actual.GroupsPeople[0].People[0].FactionID);
        Assert.AreEqual ("0 [Region 0]", actual.GroupsPeople[0].People[0].Name);
        Assert.Contains (0, actual.PeopleRolesNew[0]);
        Assert.AreEqual (0, actual.PeopleFactionsNew[0].Item2);
        Assert.AreEqual<IDType> (0, simulationVm.Context.FactionsPeople[0].ID);
        Assert.AreEqual ("Region 0", simulationVm.Context.FactionsPeople[0].Name);
        Assert.IsNull (simulationVm.Context.FactionsPeople[0].Description);
        Assert.IsTrue (simulationVm.Context.FactionsPeople[0].IsCurrency);
        Assert.IsFalse (simulationVm.Context.FactionsPeople[0].IsNotCurrency);
        Assert.AreEqual ("Currency 0", simulationVm.Context.FactionsPeople[0].Currency);
        Assert.AreEqual (1, simulationVm.Context.FactionsPeople[0].Value);
    }

    [TestMethod]
    public void Election_RunParty_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.Parties.RemoveAt (1);
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [], 1)]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        SimulationViewModel simulationVm = new (simulation, election: election);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        simulationVm.SlideCurrentIdx = title;

        ElectionViewModel actual = election.DataContext!;

        Assert.AreEqual ("Procedure 0", actual.Title);
        Assert.AreEqual ("Party 2 (2)", actual.GroupsPeople[0].Name);
        Assert.AreEqual<IDType> (2, actual.GroupsPeople[0].People[0].FactionID);
        Assert.AreEqual ("0 [2]", actual.GroupsPeople[0].People[0].Name);
        Assert.Contains (2, actual.PeopleRolesNew[0]);
        Assert.AreEqual (2, actual.PeopleFactionsNew[0].Item1);
        Assert.AreEqual<IDType> (2, simulationVm.Context.FactionsPeople[0].ID);
        Assert.AreEqual ("2", simulationVm.Context.FactionsPeople[0].Name);
        Assert.AreEqual ("Party 2", simulationVm.Context.FactionsPeople[0].Description);
        Assert.IsTrue (simulationVm.Context.FactionsPeople[0].IsCurrency);
        Assert.IsFalse (simulationVm.Context.FactionsPeople[0].IsNotCurrency);
        Assert.AreEqual ("Currency 2", simulationVm.Context.FactionsPeople[0].Currency);
        Assert.AreEqual (1, simulationVm.Context.FactionsPeople[0].Value);
    }

    [TestMethod]
    public void Election_RunNominated_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [254])]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        SimulationViewModel simulationVm = new (simulation, election: election);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        simulationVm.SlideCurrentIdx = title;

        ElectionViewModel actual = election.DataContext!;

        Assert.AreEqual ("Procedure 0", actual.Title);
        Assert.AreEqual ("Nominees", actual.GroupsPeople[0].Name);
        Assert.IsTrue (actual.GroupsPeople[0].People[0].IsCandidate);
    }

    [TestMethod]
    public void Election_RunNominatedSelected_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [254])]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ((e) => {
            e.GroupsPeople[0].People[0].IsSelected = true;
            e.RunNextElectionCommand.Execute (null);
        });
        SimulationViewModel simulationVm = new (simulation, election: election);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        simulationVm.SlideCurrentIdx = title;

        ElectionViewModel actual = election.DataContext!;

        Assert.Contains (254, actual.PeopleRolesNew[0]);
        Assert.Contains ("GH", simulationVm.Context.People[0].Roles[0].Abbreviation);
        Assert.Contains ("Government Head", simulationVm.Context.People[0].Roles[0].Name);
    }

    [TestMethod]
    public void Election_RunNominatedNotSelected_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [254])]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ((e) => {
            e.GroupsPeople[0].People[0].IsSelected = true;
            e.GroupsPeople[0].People[0].IsSelected = false;
            e.RunNextElectionCommand.Execute (null);
        });
        SimulationViewModel simulationVm = new (simulation, election: election);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        simulationVm.SlideCurrentIdx = title;

        ElectionViewModel actual = election.DataContext!;

        Assert.DoesNotContain (254, actual.PeopleRolesNew[0]);
        Assert.IsEmpty (simulationVm.Context.People[0].Roles);
    }

    [TestMethod]
    public void Election_RunAppointed_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [254], 1)]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        SimulationViewModel simulationVm = new (simulation, election: election);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        simulationVm.SlideCurrentIdx = title;

        ElectionViewModel actual = election.DataContext!;

        Assert.AreEqual ("Procedure 0", actual.Title);
        Assert.AreEqual ("Appointees", actual.GroupsPeople[0].Name);
        Assert.Contains (254, actual.PeopleRolesNew[0]);
        Assert.Contains ("GH", simulationVm.Context.People[0].Roles[0].Abbreviation);
        Assert.Contains ("Government Head", simulationVm.Context.People[0].Roles[0].Name);
    }

    [TestMethod]
    public void Election_IndependentFaction_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [
                new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [254], 1),
                new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [254], 1),
            ]
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ((e) => e.RunNextElectionCommand.Execute (null));
        FakeGenerator generator = new ([0]);
        SimulationViewModel simulationVm = new (simulation, election: election, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0"), new (1, "1")]);
        IDType title = 9;

        simulationVm.SlideCurrentIdx = title;

        Assert.AreEqual<IDType> (255, simulationVm.Context.FactionsPeople[1].ID);
        Assert.AreEqual ("Independent", simulationVm.Context.FactionsPeople[1].Name);
        Assert.IsNull (simulationVm.Context.FactionsPeople[1].Description);
        Assert.IsFalse (simulationVm.Context.FactionsPeople[1].IsCurrency);
        Assert.IsTrue (simulationVm.Context.FactionsPeople[1].IsNotCurrency);
        Assert.IsTrue (simulationVm.Context.IsCurrency);
        Assert.AreEqual ("Currency State", simulationVm.Context.CurrencyName);
        Assert.AreEqual (1, simulationVm.Context.Value);
        Assert.Contains ("GH", simulationVm.Context.FactionsPeople[1].People[0].Roles[0].Abbreviation);
        Assert.Contains ("Government Head", simulationVm.Context.FactionsPeople[1].People[0].Roles[0].Name);
    }
}
