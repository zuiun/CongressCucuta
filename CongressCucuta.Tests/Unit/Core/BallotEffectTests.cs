using CongressCucuta.Core;
using CongressCucuta.Tests.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class BallotEffectTests {
    [TestMethod]
    public void Constructor_NoTargetIDs_Throws () {
        IDType[] targetIds = [];

        Assert.Throws<ArgumentException> (() => new Ballot.Effect (0, targetIds));
    }

    [TestMethod]
    [DataRow (new byte[] { 0 })]
    [DataRow (new byte[] { 0, 0, 0 })]
    public void Constructor_ReplaceProcedureNotTwoTargetIDs_Throws (byte[] targetIds) {
        Assert.Throws<ArgumentException> (() => new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [.. targetIds]));
    }

    [TestMethod]
    public void Constructor_ModifyCurrencyZeroValue_Throws () {
        sbyte value = 0;

        Assert.Throws<ArgumentException> (() => new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [0], value));
    }

    [TestMethod]
    [DataRow (new byte[] { 2 }, "Found Party 2 (2)")]
    [DataRow (new byte[] { 3 }, "Found Party 3")]
    [DataRow (new byte[] { 2, 3 }, "Found Party 2 (2), Party 3")]
    public void ToString_FoundParty_ReturnsExpected (byte[] targetIds, string expected) {
        Ballot.Effect effect = new (Ballot.Effect.EffectType.FoundParty, [.. targetIds]);
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = simulation.Localisation;

        string actual = effect.ToString (simulation, in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { 2 }, "Dissolve Party 2 (2)")]
    [DataRow (new byte[] { 3 }, "Dissolve Party 3")]
    [DataRow (new byte[] { 2, 3 }, "Dissolve Party 2 (2), Party 3")]
    public void ToString_DissolveParty_ReturnsExpected (byte[] targetIds, string expected) {
        Ballot.Effect effect = new (Ballot.Effect.EffectType.DissolveParty, [.. targetIds]);
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = simulation.Localisation;

        string actual = effect.ToString (simulation, in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { 0 }, "Remove Procedure 0")]
    [DataRow (new byte[] { 1 }, "Remove Procedure 1")]
    [DataRow (new byte[] { 0, 1 }, "Remove Procedure 0, Procedure 1")]
    public void ToString_RemoveProcedure_ReturnsExpected (byte[] targetIds, string expected) {
        Ballot.Effect effect = new (Ballot.Effect.EffectType.RemoveProcedure, [.. targetIds]);
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = simulation.Localisation;

        string actual = effect.ToString (simulation, in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { 1, 2 }, "Replace Procedure 1 with Procedure 2:")]
    [DataRow (new byte[] { 1, 1 }, "Modify Procedure 1:")]
    public void ToString_ReplaceProcedure_ReturnsExpected (byte[] targetIds, string expected) {
        Ballot.Effect effect = new (Ballot.Effect.EffectType.ReplaceProcedure, [.. targetIds]);
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = simulation.Localisation;

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { 0 }, (sbyte) 1, "Gain 1 Currency 0")]
    [DataRow (new byte[] { 0 }, (sbyte) -1, "Lose 1 Currency 0")]
    [DataRow (new byte[] { 1 }, (sbyte) 1, "Gain 1 Currency 1")]
    [DataRow (new byte[] { 1 }, (sbyte) -1, "Lose 1 Currency 1")]
    [DataRow (new byte[] { 254 }, (sbyte) 1, "Gains 1 Currency Party")]
    [DataRow (new byte[] { 254 }, (sbyte) -1, "Loses 1 Currency Party")]
    [DataRow (new byte[] { 253 }, (sbyte) 1, "Gains 1 Currency Region")]
    [DataRow (new byte[] { 253 }, (sbyte) -1, "Loses 1 Currency Region")]
    [DataRow (new byte[] { 255 }, (sbyte) 1, "Gain 1 Currency State")]
    [DataRow (new byte[] { 255 }, (sbyte) -1, "Lose 1 Currency State")]
    public void ToString_ModifyCurrency_ReturnsExpected (byte[] targetIds, sbyte value, string expected) {
        Ballot.Effect effect = new (Ballot.Effect.EffectType.ModifyCurrency, [.. targetIds], value);
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = simulation.Localisation;

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }
}
