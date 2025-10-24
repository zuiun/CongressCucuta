using CongressCucuta.Core;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class ProcedureConfirmationTests {
    [TestMethod]
    [DataRow (0, "Always")]
    [DataRow (1, "Division of chamber")]
    [DataRow (2, "Can spend 1 Currency")]
    [DataRow (3, "Dice roll greater than or equal to 1")]
    [DataRow (4, "Can spend dice roll Currency")]
    [DataRow (5, "Declarer's dice roll greater than or equal to defender's dice roll")]
    public void ToString_Normal_ReturnsExpected (int type, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        Confirmation confirmation = new ((Confirmation.ConfirmationType) type, 1);

        string actual = confirmation.ToString ([Role.MEMBER], in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (2, "Can spend 1 Currency")]
    [DataRow (4, "Can spend dice roll Currency")]
    [DataRow (5, "Can spend declarer's dice roll Currency")]
    public void ToString_Currency_ReturnsExpected (int type, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        Confirmation confirmation = new ((Confirmation.ConfirmationType) type, 1);

        string actual = confirmation.ToString ([Role.MEMBER, Role.LEADER_PARTY, Role.LEADER_REGION, 0], in localisation);

        Assert.Contains (expected, actual);
        Assert.Contains ("Currency State", actual);
        Assert.Contains ("Currency Region", actual);
        Assert.Contains ("Currency Party", actual);
        Assert.Contains ("Currency 0", actual);
    }

    [TestMethod]
    public void ToString_AdversarialNoCurrency_ReturnsExpected () {
        Localisation localisation = FakeLocalisation.Create ();
        localisation.Currencies.Clear ();
        Confirmation confirmation = new (Confirmation.ConfirmationType.DiceAdversarial);
        string unexpected = "Can spend declarer's dice roll Currency";

        string actual = confirmation.ToString ([Role.MEMBER, Role.LEADER_PARTY, Role.LEADER_REGION, 0], in localisation);

        Assert.DoesNotContain (unexpected, actual);
        Assert.DoesNotContain ("Currency State", actual);
        Assert.DoesNotContain ("Currency Region", actual);
        Assert.DoesNotContain ("Currency Party", actual);
        Assert.DoesNotContain ("Currency 0", actual);
    }
}
