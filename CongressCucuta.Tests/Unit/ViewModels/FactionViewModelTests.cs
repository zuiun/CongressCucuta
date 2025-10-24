using CongressCucuta.Core;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class FactionViewModelTests {
    [TestMethod]
    public void ReplaceParty_Abbreviation_MutatesExpected () {
        Localisation localisation = FakeLocalisation.Create ();
        localisation.Parties[0] = ("Party 1", []);
        localisation.Abbreviations[0] = "P1";
        FactionViewModel faction = new (0, "Party 0");

        faction.ReplaceParty (in localisation);

        Assert.AreEqual ("P1", faction.Name);
        Assert.AreEqual ("Party 1", faction.Description);
    }

    [TestMethod]
    public void ReplaceParty_NoAbbreviation_MutatesExpected () {
        Localisation localisation = FakeLocalisation.Create ();
        localisation.Parties[0] = ("Party 1", []);
        FactionViewModel faction = new (0, "Party 0");

        faction.ReplaceParty (in localisation);

        Assert.AreEqual ("Party 1", faction.Name);
        Assert.IsNull (faction.Description);
    }
}
