using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class DeclareViewModelTests {
    [TestMethod]
    public void Constructor_Procedures_ConstructsExpected () {
        FakeSimulation simulation = new ();
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);

        IDType expected = 3;
        DeclareViewModel actual = new (0, context, in localisation);

        Assert.AreEqual (expected, actual.Procedures[0].ID);
        Assert.AreEqual ("Procedure 3", actual.Procedures[0].Name);
    }

    [TestMethod]
    public void Constructor_NotAllowed_ConstructsExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [1]
        );
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);

        string expected = "Not allowed";
        DeclareViewModel actual = new (0, context, in localisation);

        Assert.IsFalse (actual.Procedures[0].IsActive);
        Assert.AreEqual (expected, actual.Procedures[0].Error);
    }

    [TestMethod]
    public void Constructor_AlreadyDeclared_ConstructsExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.Context.ProceduresDeclared.Add (3);

        string expected = "Already declared";
        DeclareViewModel actual = new (0, context, in localisation);

        Assert.IsFalse (actual.Procedures[0].IsActive);
        Assert.AreEqual (expected, actual.Procedures[0].Error);
    }

    [TestMethod]
    public void Constructor_InsufficientCurrency_ConstructsExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresDeclared[0] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.CurrencyValue, 2),
            []
        );
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);

        string expected = "Insufficient Currency State";
        DeclareViewModel actual = new (0, context, in localisation);

        Assert.IsFalse (actual.Procedures[0].IsActive);
        Assert.AreEqual (expected, actual.Procedures[0].Error);
    }

    [TestMethod]
    public void DeclareProcedureCommand_ExecuteManual_MutatesExpected () {
        FakeSimulation simulation = new ();
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        DeclareViewModel declare = new (0, context, in localisation);
        declare.ConfirmingProcedure += Declare_ConfirmingProcedureEventHandler;

        declare.Procedures[0].DeclareProcedureCommand.Execute (null);

        static void Declare_ConfirmingProcedureEventHandler (ConfirmingProcedureEventArgs e) {
            Assert.AreEqual<IDType> (0, e.PersonID);
            Assert.AreEqual<IDType> (3, e.ProcedureID);
            e.IsManual = true;
        }
        Assert.IsTrue (declare.IsManual);
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void DeclareProcedureCommand_Execute_MutatesExpected (bool isConfirmed) {
        FakeSimulation simulation = new ();
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        DeclareViewModel declare = new (0, context, in localisation);
        declare.ConfirmingProcedure += Declare_ConfirmingProcedureEventHandler;

        declare.Procedures[0].DeclareProcedureCommand.Execute (null);

        void Declare_ConfirmingProcedureEventHandler (ConfirmingProcedureEventArgs e) {
            Assert.AreEqual<IDType> (0, e.PersonID);
            Assert.AreEqual<IDType> (3, e.ProcedureID);
            e.IsConfirmed = isConfirmed;
            e.Message = "Message";
        }
        Assert.AreEqual (isConfirmed, declare.IsSuccess);
        Assert.IsTrue (declare.IsConfirmation);
        Assert.AreEqual ("Message", declare.Message);
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void DeclareProcedureCommand_CanExecute_ReturnsExpected (bool isActive) {
        FakeSimulation simulation = new ();
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        DeclareViewModel declare = new (0, context, in localisation);
        declare.Procedures[0].IsActive = isActive;

        bool actual = declare.Procedures[0].DeclareProcedureCommand.CanExecute (null);

        Assert.AreEqual (isActive, actual);
    }

    [TestMethod]
    public void ConfirmProcedureCommand_Execute_MutatesExpected () {
        FakeSimulation simulation = new ();
        Localisation localisation = simulation.Localisation;
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        DeclareViewModel declare = new (0, context, in localisation);
        declare.ConfirmingProcedure += Declare_ConfirmingProcedureEventHandler;

        declare.ConfirmProcedureCommand.Execute (null);

        static void Declare_ConfirmingProcedureEventHandler (ConfirmingProcedureEventArgs e) {
            Assert.AreEqual<IDType> (0, e.PersonID);
            e.IsConfirmed = true;
            e.Message = "Message";
        }
        Assert.IsTrue (declare.IsSuccess);
        Assert.IsTrue (declare.IsConfirmation);
        Assert.AreEqual ("Message", declare.Message);
    }
}
