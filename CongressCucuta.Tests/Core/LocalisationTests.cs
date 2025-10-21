using CongressCucuta.Core;
using CongressCucuta.Tests.Unit.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class LocalisationTests {
    [TestMethod]
    [DataRow ((byte) 0, "Region 0")]
    [DataRow ((byte) 1, "Region 1")]
    [DataRow ((byte) 2, "Party 2 (2)")]
    [DataRow ((byte) 3, "Party 3")]
    public void GetFactionAndAbbreviation_Localisation_ReturnsExpected (byte factionId, string expected) {
        Localisation localisation = FakeLocalisation.Create ();

        string actual = localisation.GetFactionAndAbbreviation (factionId);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void GetFactionAndAbbreviation_LocalisationWrongID_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => localisation.GetFactionAndAbbreviation (255));
    }

    [TestMethod]
    [DataRow ((byte) 0, "Region 0")]
    [DataRow ((byte) 1, "Region 1")]
    [DataRow ((byte) 2, "2")]
    [DataRow ((byte) 3, "Party 3")]
    public void GetFactionOrAbbreviation_Localisation_ReturnsExpected (byte factionId, string expected) {
        Localisation localisation = FakeLocalisation.Create ();

        string actual = localisation.GetFactionOrAbbreviation (factionId);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void GetFactionOrAbbreviation_LocalisationWrongID_Throws () {
        Localisation localisation = FakeLocalisation.Create ();

        Assert.Throws<ArgumentException> (() => localisation.GetFactionOrAbbreviation (255));
    }
}
