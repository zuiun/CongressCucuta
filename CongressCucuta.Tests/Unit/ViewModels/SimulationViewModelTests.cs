using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class SimulationViewModelTests {
    private static bool Contains (SlideViewModel slide, params string[] description)
        => description.All (d => slide.Description.Any (l => l.Text.Contains (d)));

    [TestMethod]
    public void SetSlideCurrentIdx_Title_InvokesExpected () {
        FakeSimulation simulation = new ();
        simulation.ProceduresGovernmental[0] = new (
            0,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanVote, [255], 0)]
        );
        SimulationViewModel vm = new (simulation);
        vm.InitialisePeople ([new (0, "0")]);
        IDType title = 9;

        vm.SlideCurrentIdx = title;

        Assert.IsFalse (vm.Context.People[0].CanVote);
    }

    [TestMethod]
    [DataRow ((byte) 10)]
    [DataRow ((byte) 11)]
    public void SetSlideCurrentIdx_StartBallot_InvokesExpected (byte slideId) {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);

        vm.SlideCurrentIdx = slideId;

        Assert.IsTrue (vm.Context.IsBallotCount);
    }

    [TestMethod]
    public void SetSlideCurrentIdx_EndBallot_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType ballot0 = 10;
        vm.SlideCurrentIdx = ballot0;

        vm.SlideCurrentIdx = 12;

        Assert.IsFalse (vm.Context.IsBallotCount);
    }

    [TestMethod]
    public void Constructor_Introduction_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 0;
        string title = vm.Localisation.State;
        string description = vm.Localisation.Government;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual (title, actual.Title);
        Assert.IsFalse (actual.IsContent);
        Assert.IsTrue (Contains (actual, description));
        Assert.IsFalse (actual.Description[0].IsContent);
        Assert.AreEqual<IDType> (1, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_Context_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 1;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Context", actual.Title);
        Assert.AreEqual<IDType> (0, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (2, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_ProcedureGovernmental_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 2;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Governmental Procedures", actual.Title);
        Assert.IsTrue (Contains (actual, "Procedure 0"));
        Assert.IsNotNull (actual.Description[0].Description);
        Assert.AreEqual<IDType> (1, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (3, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_ProcedureSpecial_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 3;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Special Procedures", actual.Title);
        Assert.IsTrue (Contains (actual, "Procedure 1"));
        Assert.IsNotNull (actual.Description[0].Description);
        Assert.AreEqual<IDType> (2, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (4, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_ProcedureDeclared_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 4;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Declared Procedures", actual.Title);
        Assert.IsTrue (Contains (actual, "Procedure 3"));
        Assert.IsNotNull (actual.Description[0].Description);
        Assert.AreEqual<IDType> (3, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (5, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Roles_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 5;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Roles", actual.Title);
        Assert.IsTrue (Contains (
            actual,
            "Speaker",
            "Member",
            "Government Head",
            "State Head",
            "Party Leader",
            "Region Leader",
            "0 Leader",
            "1 Leader",
            "2 Leader",
            "3 Leader"
        ));
        Assert.AreEqual<IDType> (4, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (6, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Regions_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 6;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Regions", actual.Title);
        Assert.IsTrue (Contains (actual, "Region 0", "Region 1"));
        Assert.AreEqual<IDType> (5, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (7, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Parties_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 7;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Parties", actual.Title);
        Assert.IsTrue (Contains (actual, "Party 2 (2)", "Party 3"));
        Assert.AreEqual<IDType> (6, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (8, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Ballots_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 8;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Ballots", actual.Title);
        Assert.IsTrue (Contains (actual, "0", "Ballot 0"));
        Assert.IsFalse (Contains (actual, "1", "Incident 1"));
        Assert.AreEqual<IDType> (7, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
        Assert.AreEqual<IDType> (9, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Title_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 9;
        string title = vm.Localisation.Period;
        string date = vm.Localisation.Date;
        string situation = vm.Localisation.Situation;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual (title, actual.Title);
        Assert.IsFalse (actual.IsContent);
        Assert.IsTrue (Contains (actual, date, situation));
        Assert.IsFalse (actual.Description[0].IsContent);
        Assert.IsFalse (actual.Description[1].IsContent);
        Assert.AreEqual<IDType> (10, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_Ballot0_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 10;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("0", actual.Title);
        Assert.IsTrue (Contains (actual, "Ballot 0"));
        Assert.AreEqual<IDType> (12, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Pass", actual.Links[0].Name);
        Assert.AreEqual<IDType> (13, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Fail", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Ballot1_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 11;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("1", actual.Title);
        Assert.IsTrue (Contains (actual, "Incident 1"));
        Assert.AreEqual<IDType> (14, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Pass", actual.Links[0].Name);
        Assert.AreEqual<IDType> (15, actual.Links[1].Link.TargetID);
        Assert.AreEqual ("Fail", actual.Links[1].Name);
    }

    [TestMethod]
    public void Constructor_Ballot0Pass_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 12;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("0 Passed", actual.Title);
        Assert.IsTrue (Contains (actual, "Replace Procedure 1 with Procedure 2:"));
        Assert.IsTrue (actual.Description[0].IsImportant);
        Assert.AreEqual<IDType> (11, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_Ballot0Fail_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 13;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("0 Failed", actual.Title);
        Assert.AreEqual<IDType> (16, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_Ballot1Pass_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 14;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("1 Passed", actual.Title);
        Assert.AreEqual<IDType> (16, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_Ballot1Fail_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 15;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("1 Failed", actual.Title);
        Assert.AreEqual<IDType> (16, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_Result_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 16;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Result", actual.Title);
        Assert.AreEqual<IDType> (17, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_HistoricalResults_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 17;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("Historical Results", actual.Title);
        Assert.IsTrue (Contains (actual, "Passed", "0", "Failed", "1 (Procedure 3)"));
        Assert.AreEqual<IDType> (18, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Next", actual.Links[0].Name);
    }

    [TestMethod]
    public void Constructor_End_ConstructsExpected () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        IDType id = 18;

        SlideViewModel actual = vm.Slides[id];

        Assert.AreEqual (id, actual.ID);
        Assert.AreEqual ("End", actual.Title);
        Assert.AreEqual<IDType> (17, actual.Links[0].Link.TargetID);
        Assert.AreEqual ("Previous", actual.Links[0].Name);
    }

    [TestMethod]
    public void SwitchSlideCommand_Execute_MutatesExpected () {
        IDType expected = 1;
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        Link<SlideViewModel> link = new (new AlwaysCondition (), expected);

        vm.SwitchSlideCommand.Execute (link);

        Assert.AreEqual (expected, vm.Slide.ID);
        Assert.AreEqual (expected, vm.SlideCurrentIdx);
    }

    [TestMethod]
    public void SwitchSlideCommand_CanExecuteTrue_ReturnsTrue () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        Link<SlideViewModel> link = new (new AlwaysCondition (), 0);

        bool actual = vm.SwitchSlideCommand.CanExecute (link);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void SwitchSlideCommand_CanExecuteFalse_ReturnsFalse () {
        FakeSimulation simulation = new ();
        SimulationViewModel vm = new (simulation);
        Link<SlideViewModel> link = new (new NeverCondition (), 0);

        bool actual = vm.SwitchSlideCommand.CanExecute (link);

        Assert.IsFalse (actual);
    }
}
