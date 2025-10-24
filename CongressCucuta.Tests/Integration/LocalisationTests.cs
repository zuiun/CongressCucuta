using CongressCucuta.Core;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;
using CongressCucuta.Views;

namespace CongressCucuta.Tests.Integration;

[TestClass]
public sealed class LocalisationTests {
    [TestMethod]
    public void Localisation_ReplacePartyResult_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [2, 3]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [2], 1),
                ],
                []
            ),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [2], 1)], [])
        );
        simulation.Ballots[1] = new (
            1,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [2], 1)], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [2], 1)], [])
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        IDType ballot0Pass = 12;
        IDType ballot0Fail = 13;
        IDType incident1Pass = 14;
        IDType incident1Fail = 15;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.AreEqual ("2:", simulationVm.Slides[ballot0Pass].Description[1].Text);
        Assert.AreEqual ("2:", simulationVm.Slides[ballot0Fail].Description[0].Text);
        Assert.AreEqual ("Party 3:", simulationVm.Slides[incident1Pass].Description[0].Text);
        Assert.AreEqual ("Party 3:", simulationVm.Slides[incident1Fail].Description[0].Text);
    }

    [TestMethod]
    public void Localisation_ReplacePartyProcedures_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (0, [new (Procedure.Effect.EffectType.PermissionsCanVote, [2], 0)]);
        simulation.ProceduresSpecial[0] = new (1, [new (Procedure.Effect.EffectType.CurrencyAdd, [2], 1)], []);
        simulation.ProceduresDeclared[0] = new (
            3,
            [
                new (Procedure.Effect.EffectType.BallotPass, []),
                new (Procedure.Effect.EffectType.CurrencyAdd, [2], 1),
            ],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [2, 3])], []),
            new Ballot.Result ([], [])
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.Contains ("3 Leaders", simulationVm.Context.ProceduresGovernmental[0].Effects);
        Assert.Contains ("Party 3", simulationVm.Context.ProceduresSpecial[0].Effects);
        Assert.Contains ("Party 3", simulationVm.Context.ProceduresDeclared[0].Effects);
    }

    [TestMethod]
    public void Localisation_ReplacePartyFactionsAbbreviation_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.Localisation.Abbreviations[3] = "3";
        simulation.ProceduresGovernmental[0] = new (0, [new (Procedure.Effect.EffectType.ElectionParty, [], 1)]);
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [2, 3])], []),
            new Ballot.Result ([], [])
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, election, declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.FactionsPeople[0].People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.AreEqual ("3", simulationVm.Context.FactionsPeople[0].Name);
        Assert.AreEqual ("Party 3", simulationVm.Context.FactionsPeople[0].Description);
    }

    [TestMethod]
    public void Localisation_ReplacePartyFactionsNoAbbreviation_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (0, [new (Procedure.Effect.EffectType.ElectionParty, [], 1)]);
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [2, 3])], []),
            new Ballot.Result ([], [])
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, election, declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.FactionsPeople[0].People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.AreEqual ("Party 3", simulationVm.Context.FactionsPeople[0].Name);
        Assert.IsNull (simulationVm.Context.FactionsPeople[0].Description);
    }

    [TestMethod]
    public void Localisation_ReplacePartyRoles_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (0, [new (Procedure.Effect.EffectType.ElectionParty, [], 1)]);
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        simulation.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [2, 3])], []),
            new Ballot.Result ([], [])
        );
        FakeWindow<ElectionWindow, ElectionViewModel> election = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, election, declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;
        IDType ballot0 = 10;
        simulationVm.SlideCurrentIdx = title;
        simulationVm.SlideCurrentIdx = ballot0;
        simulationVm.Context.FactionsPeople[0].People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.AreEqual ("3 Leader", simulationVm.Context.FactionsPeople[0].People[0].Roles[0].Name);
        Assert.Contains ("3L", simulationVm.Context.FactionsPeople[0].People[0].Roles[0].Abbreviation);
    }
}
