using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Unit.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class SimulationTests {
    [TestMethod]
    public void Validate_1_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.RolesPermissions.Clear ();

        string expected = "There must be a MEMBER Role";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_2_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.RolesPermissions[100] = new ();

        string expected = "Role ID 100 does not correspond with any Faction ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_3_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.CurrenciesValues[100] = -1;

        string expected = "Currency ID 100 does not correspond with any Faction ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_4RegionPartyOverlap_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Parties.Add (new (0));

        string expected = "Region ID 0 overlaps with Party ID 0";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_4RegionOverlap_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Regions.Add (new (0));

        string expected = "Region ID 0 is repeated";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_4PartyOverlap_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Parties.Add (new (2));

        string expected = "Party ID 2 is repeated";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_5BallotPass_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Pass.Links.Add (new (new AlwaysCondition (), 100));

        string expected = "Ballot ID 0 Pass Link targeting 100 does not correspond with any Ballot ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_5BallotFail_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Links.Add (new (new AlwaysCondition (), 100));

        string expected = "Ballot ID 0 Fail Link targeting 100 does not correspond with any Ballot ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_5Result_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Results[0].Links.Add (new (new AlwaysCondition (), 100));

        string expected = "Result ID 0 Link targeting 100 does not correspond with any Result ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureGovernmental_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresGovernmental.Add (new (0, [new (Procedure.Effect.EffectType.ElectionParty, [100])]));

        string expected = "ProcedureImmediate ID 0 targets an invalid Role ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureSpecialFilter_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.Add (new (0, [new (Procedure.Effect.EffectType.CurrencyInitialise, [])], [100]));

        string expected = "ProcedureTargeted ID 0 filters an invalid Ballot ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureSpecialCurrencyAdd_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.Add (new (0, [new (Procedure.Effect.EffectType.CurrencyAdd, [100])], []));

        string expected = "ProcedureTargeted ID 0 targets an invalid Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureSpecialCurrencyRemove_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.Add (new (0, [new (Procedure.Effect.EffectType.CurrencySubtract, [100])], []));

        string expected = "ProcedureTargeted ID 0 targets an invalid Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureSpecialProcedureActivate_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.Add (new (0, [new (Procedure.Effect.EffectType.ProcedureActivate, [100])], []));

        string expected = "ProcedureTargeted ID 0 targets an invalid ProcedureImmediate ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureDeclaredFilter_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresDeclared.Add (new (0, [new (Procedure.Effect.EffectType.BallotPass, [])], new (), [100]));

        string expected = "ProcedureDeclared ID 0 filters an invalid Role ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureDeclaredCurrencyAdd_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresDeclared.Add (new (0, [new (Procedure.Effect.EffectType.CurrencyAdd, [100])], new (), []));

        string expected = "ProcedureDeclared ID 0 targets an invalid Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureDeclaredCurrencySubtract_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresDeclared.Add (new (0, [new (Procedure.Effect.EffectType.CurrencySubtract, [100])], new (), []));

        string expected = "ProcedureDeclared ID 0 targets an invalid Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_6ProcedureDeclaredTarget_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresDeclared.Add (new (0, [new (Procedure.Effect.EffectType.ElectionParty, [100])], new (), []));

        string expected = "ProcedureDeclared ID 0 targets an invalid Role ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_7Region_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Regions.Add (new (4));

        string expected = "Region ID 4 does not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_7Party_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Parties.Add (new (4));

        string expected = "Party ID 4 does not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8Role_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Roles.Remove (Role.MEMBER);

        string expected = "Role ID 255 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8Region_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Regions.Remove (0);

        string expected = "Region ID 0 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8Party_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Parties.Remove (2);

        string expected = "Party ID 2 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8Currency_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Currencies.Remove (0);

        string expected = "Currency ID 0 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8ProcedureGovernmental_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Procedures.Remove (0);

        string expected = "ProcedureImmediate ID 0 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8ProcedureSpecial_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Procedures.Remove (1);

        string expected = "ProcedureTargeted ID 1 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8ProcedureDeclared_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Procedures.Remove (3);

        string expected = "ProcedureDeclared ID 3 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8Ballot_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Ballots.Remove (0);

        string expected = "Ballot ID 0 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_8Result_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Localisation.Results.Remove (0);

        string expected = "Result ID 0 does not have a Localisation entry";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9Region_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Regions.Add (new (5));
        simulation.CurrenciesValues[5] = 1;
        simulation.Localisation.Regions[5] = (Localisation.UNUSED, []);
        simulation.Localisation.Currencies[5] = Localisation.UNUSED;

        string expected = "Region ID 5 does not match its offset index 2 in Regions";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9Party_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Parties.Add (new (6));
        simulation.CurrenciesValues[6] = 1;
        simulation.Localisation.Parties[6] = (Localisation.UNUSED, []);
        simulation.Localisation.Currencies[6] = Localisation.UNUSED;

        string expected = "Party ID 6 does not match its offset index 4 in Parties";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9PartyRegion_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Parties.RemoveAt (0);
        simulation.RolesPermissions.Remove (2);
        simulation.CurrenciesValues.Remove (2);

        string expected = "Region and Party IDs are not contiguous";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9ProcedureGovernmental_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresGovernmental.Add (new (0, [new (Procedure.Effect.EffectType.ElectionParty, [])]));

        string expected = "ProcedureImmediate ID 0 does not match its index 1 in ProceduresGovernmental";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9ProcedureSpecial_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.Add (new (0, [new (Procedure.Effect.EffectType.CurrencyInitialise, [])], []));

        string expected = "ProcedureTargeted ID 0 does not match its offset index 3 in ProceduresSpecial";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9ProcedureDeclared_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresDeclared.Add (new (
            0,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new (Confirmation.ConfirmationType.Always),
            []
        ));

        string expected = "ProcedureDeclared ID 0 does not match its offset index 4 in ProceduresDeclared";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9Ballot_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots.Add (new (0, new ([], []), new ([], [])));

        string expected = "Ballot ID 0 does not match its index 2 in Ballots";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_9Result_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Results.Add (new (0, []));

        string expected = "Result ID 0 does not match its index 1 in Results";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_10NoProcedure_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresGovernmental.Clear ();
        simulation.ProceduresSpecial.Clear ();
        simulation.ProceduresDeclared.Clear ();

        string expected = "If there are Currencies, then the first ProcedureImmediate must have Action CurrencyInitialise";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_10Effect_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.RemoveAt (0);
        simulation.ProceduresSpecial.Insert (0, new (1, [new (Procedure.Effect.EffectType.PermissionsCanVote, [])], []));

        string expected = "If there are Currencies, then the first ProcedureImmediate must have Action CurrencyInitialise";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_10Duplicate_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.ProceduresSpecial.RemoveAt (1);
        simulation.ProceduresSpecial.Add (new (2, [new (Procedure.Effect.EffectType.CurrencyInitialise, [])], []));

        string expected = "Only the first ProcedureImmediate may have Action CurrencyInitialise";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11PassFoundParty_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Pass.Effects.Add (new (Ballot.Effect.EffectType.FoundParty, [100]));

        string expected = "Ballot ID 0 Pass Effect Target IDs do not correspond with any Party ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11PassDissolveParty_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Pass.Effects.Add (new (Ballot.Effect.EffectType.DissolveParty, [100]));

        string expected = "Ballot ID 0 Pass Effect Target IDs do not correspond with any Party ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11PassRemoveProcedure_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Pass.Effects.Add (new (Ballot.Effect.EffectType.RemoveProcedure, [100]));

        string expected = "Ballot ID 0 Pass Effect Target IDs do not correspond with any Procedure ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11PassReplaceProcedure_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Pass.Effects.Add (new (Ballot.Effect.EffectType.ReplaceProcedure, [100, 100]));

        string expected = "Ballot ID 0 Pass Effect Target IDs do not correspond with any Procedure ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11PassModifyCurrency_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Pass.Effects.Add (new (Ballot.Effect.EffectType.ModifyCurrency, [100], 1));

        string expected = "Ballot ID 0 Pass Effect Target IDs do not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailFoundParty_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.FoundParty, [100]));

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Party ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailDissolveParty_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.DissolveParty, [100]));

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Party ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailRemoveProcedure_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.RemoveProcedure, [100]));

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Procedure ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailReplaceProcedure_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.ReplaceProcedure, [100, 100]));

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Procedure ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailModifyCurrencyState_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.ModifyCurrency, [255], 1));
        simulation.CurrenciesValues.Remove (255);

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailModifyCurrencyParty_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.ModifyCurrency, [254], 1));
        simulation.CurrenciesValues.Remove (2);

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailModifyCurrencyRegion_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.ModifyCurrency, [253], 1));
        simulation.CurrenciesValues.Remove (0);

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_11FailModifyCurrencyOther_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.Ballots[0].Fail.Effects.Add (new (Ballot.Effect.EffectType.ModifyCurrency, [100], 1));

        string expected = "Ballot ID 0 Fail Effect Target IDs do not correspond with any Currency ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_12FailRegion_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.RolesPermissions.Remove (Role.LEADER_REGION);

        string expected = "LEADER_REGION Role must exist when any Region Role exists";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_12FailParty_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.RolesPermissions.Remove (Role.LEADER_PARTY);

        string expected = "LEADER_PARTY Role must exist when any Party Role exists";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_13FailBallotPassed_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.History.BallotsPassed.Add (100);

        string expected = "History Ballot ID 100 does not correspond with any Ballot ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_13FailNoBallotProcedureDeclared_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.History.BallotsProceduresDeclared[100] = [];

        string expected = "History Ballot ID 100 does not correspond with any Ballot ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }

    [TestMethod]
    public void Validate_13FailBallotNoProcedureDeclared_Throws () {
        Simulation simulation = new FakeSimulation ();
        simulation.History.BallotsProceduresDeclared[1] = [100];

        string expected = "History ProcedureDeclared ID 100 does not correspond with any ProcedureDeclared ID";

        var e = Assert.Throws<ArgumentException> (() => new Simulation (
            simulation.History,
            simulation.RolesPermissions,
            simulation.Regions,
            simulation.Parties,
            simulation.CurrenciesValues,
            simulation.ProceduresGovernmental,
            simulation.ProceduresSpecial,
            simulation.ProceduresDeclared,
            simulation.Ballots,
            simulation.Results,
            simulation.Localisation
        ));
        Assert.AreEqual (expected, e.Message);
    }
}
