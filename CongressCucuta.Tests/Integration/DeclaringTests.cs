using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;
using CongressCucuta.Views;

namespace CongressCucuta.Tests.Integration;

[TestClass]
public sealed class DeclaringTests {
    [TestMethod]
    public void Declaring_Always_MutatesExpected () {
        FakeSimulation simulation = new ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Success", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DivisionChamberManual_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsManual);
    }

    [TestMethod]
    public void Declaring_DivisionChamberSuccess_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            []
        );
        simulation.CurrenciesValues[255] = 10;
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0, -1]);
        SimulationViewModel simulationVm = new (simulation, declare: declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;
        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        declareVm.ConfirmProcedureCommand.Execute (null);

        Assert.IsFalse (declareVm.IsManual);
        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Success", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_CurrencyValue_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.CurrencyValue, 0),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Success: Spent 0 Currency State", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceValueSuccess_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceValue, 0),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.Contains ("Success: Rolled", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceValueFailure_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceValue, 10),
            []
        );
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.Contains ("Failure: Rolled", declareVm.Message);
        Assert.Contains (", but needed at least 10", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceCurrencySuccess_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceCurrency),
            []
        );
        simulation.CurrenciesValues[255] = 10;
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.Contains ("Success: Rolled and spent", declareVm.Message);
        Assert.Contains ("Currency State", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceCurrencyFailure_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceCurrency),
            []
        );
        simulation.CurrenciesValues[255] = -1;
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.Contains ("Failure: Rolled", declareVm.Message);
        Assert.Contains (", but only had -1 Currency State", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceAdversarialCurrencyFailure_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );
        simulation.CurrenciesValues[255] = -1;
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        SimulationViewModel simulationVm = new (simulation, declare: declare);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.Contains ("Failure: Rolled", declareVm.Message);
        Assert.Contains (", but only had -1 Currency State", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceAdversarialDefenderFailure_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );
        simulation.CurrenciesValues[255] = 10;
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0, 6]);
        SimulationViewModel simulationVm = new (simulation, declare: declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Failure: Rolled and spent 0 Currency State, but defender rolled 6", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceAdversarialSuccess_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );
        simulation.CurrenciesValues[255] = 10;
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0, -1]);
        SimulationViewModel simulationVm = new (simulation, declare: declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Success: Rolled and spent 0 Currency State, while defender rolled -1", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceAdversarialNoCurrencyFailure_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );
        simulation.CurrenciesValues.Clear ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0, 1]);
        SimulationViewModel simulationVm = new (simulation, declare: declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsFalse (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Failure: Rolled 0, but defender rolled 1", declareVm.Message);
    }

    [TestMethod]
    public void Declaring_DiceAdversarialNoCurrencySuccess_MutatesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );
        simulation.CurrenciesValues.Clear ();
        FakeWindow<DeclareWindow, DeclareViewModel> declare = new ();
        FakeGenerator generator = new ([0, -1]);
        SimulationViewModel simulationVm = new (simulation, declare: declare, generator: generator);
        simulationVm.InitialisePeople ([new (0, "0")]);
        simulationVm.Context.People[0].DeclareProcedureCommand.Execute (null);
        DeclareViewModel declareVm = declare.DataContext!;

        declareVm.Procedures[0].DeclareProcedureCommand.Execute (null);

        Assert.IsTrue (declareVm.IsSuccess);
        Assert.IsTrue (declareVm.IsConfirmation);
        Assert.AreEqual ("Success: Rolled 0, while defender rolled -1", declareVm.Message);
    }
}
