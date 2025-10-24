using CongressCucuta.Core;
using CongressCucuta.Tests.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class LocalisationTests {
    [TestMethod]
    public void Constructor_StateEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            "",
            localisation.Government,
            localisation.Context,
            localisation.Date,
            localisation.Situation,
            localisation.Period,
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_GovernmentEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            "",
            localisation.Context,
            localisation.Date,
            localisation.Situation,
            localisation.Period,
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_ContextEmptyOuter_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            localisation.Government,
            [],
            localisation.Date,
            localisation.Situation,
            localisation.Period,
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_ContextEmptyInner_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            localisation.Government,
            [""],
            localisation.Date,
            localisation.Situation,
            localisation.Period,
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_DateEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            localisation.Government,
            localisation.Context,
            "",
            localisation.Situation,
            localisation.Period,
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_SituationEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            localisation.Government,
            localisation.Context,
            localisation.Date,
            "",
            localisation.Period,
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_PeriodEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            localisation.Government,
            localisation.Context,
            localisation.Date,
            localisation.Situation,
            "",
            localisation.Roles,
            localisation.Speaker,
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    public void Constructor_SpeakerEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => new Localisation (
            localisation.State,
            localisation.Government,
            localisation.Context,
            localisation.Date,
            localisation.Situation,
            localisation.Period,
            localisation.Roles,
            "",
            localisation.Region,
            localisation.Regions,
            localisation.Party,
            localisation.Parties,
            localisation.Abbreviations,
            localisation.Currencies,
            localisation.Procedures,
            localisation.Ballots,
            localisation.Results
        ));
    }

    [TestMethod]
    [DataRow ((byte) 0, "Region 0")]
    [DataRow ((byte) 1, "Region 1")]
    [DataRow ((byte) 2, "Party 2 (2)")]
    [DataRow ((byte) 3, "Party 3")]
    public void GetFactionAndAbbreviation_Normal_ReturnsExpected (byte factionId, string expected) {
        Localisation localisation = FakeLocalisation.Create ();

        string actual = localisation.GetFactionAndAbbreviation (factionId);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void GetFactionAndAbbreviation_NoFaction_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => localisation.GetFactionAndAbbreviation (255));
    }

    [TestMethod]
    [DataRow ((byte) 0, "Region 0")]
    [DataRow ((byte) 1, "Region 1")]
    [DataRow ((byte) 2, "2")]
    [DataRow ((byte) 3, "Party 3")]
    public void GetFactionOrAbbreviation_Normal_ReturnsExpected (byte factionId, string expected) {
        Localisation localisation = FakeLocalisation.Create ();

        string actual = localisation.GetFactionOrAbbreviation (factionId);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void GetFactionOrAbbreviation_NoFaction_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => localisation.GetFactionOrAbbreviation (255));
    }

    [TestMethod]
    [DataRow ((byte) 255, (byte) 3)]
    [DataRow ((byte) 2, (byte) 255)]
    public void ReplaceParty_NoParty_Throws (byte originalId, byte newId) {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => localisation.ReplaceParty (originalId, newId));
    }

    [TestMethod]
    public void ReplaceParty_Abbreviation_MutatesExpected () {
        Localisation localisation = FakeLocalisation.Create ();

        localisation.ReplaceParty (3, 2);

        Assert.AreEqual ("Party 2", localisation.Parties[3].Item1);
        Assert.AreEqual ("2", localisation.Abbreviations[3]);
    }

    [TestMethod]
    public void ReplaceParty_NoAbbreviation_MutatesExpected () {
        Localisation localisation = FakeLocalisation.Create ();

        localisation.ReplaceParty (2, 3);

        Assert.AreEqual ("Party 3", localisation.Parties[2].Item1);
        Assert.DoesNotContain (2, localisation.Abbreviations.Keys);
    }

    [TestMethod]
    public void ReplaceParty_Role_MutatesExpected () {
        Localisation localisation = FakeLocalisation.Create ();

        localisation.ReplaceParty (2, 3);

        Assert.AreEqual ("3 Leader", localisation.Roles[2].Item1);
    }

    [TestMethod]
    public void ReplaceParty_NoRole_MutatesExpected () {
        Localisation localisation = FakeLocalisation.Create ();
        localisation.Roles.Clear ();

        localisation.ReplaceParty (2, 3);

        Assert.DoesNotContain (2, localisation.Roles.Keys);
    }

    [TestMethod]
    public void ReplaceParty_RoleNoRole_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        localisation.Roles.Remove (3);

        Assert.Throws<ArgumentException> (() => localisation.ReplaceParty (2, 3));
    }
}
