using CongressCucuta.Core;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Unit.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class ProcedureTests {
    [TestMethod]
    public void Constructor_ProcedureNoEffect_Throws () {
        Assert.Throws<ArgumentException> (() => new ProcedureImmediate (0, []));
        Assert.Throws<ArgumentException> (() => new ProcedureTargeted (0, [], []));
        Assert.Throws<ArgumentException> (() => new ProcedureDeclared (0, [], new Confirmation (), []));
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (1)]
    [DataRow (2)]
    [DataRow (3)]
    [DataRow (4)]
    [DataRow (5)]
    [DataRow (6)]
    [DataRow (11)]
    [DataRow (12)]
    [DataRow (13)]
    public void Constructor_ProcedureImmediateWrongType_Throws (int type) {
        Procedure.Effect effect = new ((Procedure.Effect.EffectType) type, [0]);

        Assert.Throws<ArgumentException> (() => new ProcedureImmediate (0, [effect]));
    }

    [TestMethod]
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void Constructor_ProcedureImmediateNoTarget_Throws (int type) {
        Procedure.Effect effect = new ((Procedure.Effect.EffectType) type, []);

        Assert.Throws<ArgumentException> (() => new ProcedureImmediate (0, [effect]));
    }

    [TestMethod]
    [DataRow (7)]
    [DataRow (8)]
    [DataRow (9)]
    [DataRow (10)]
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void YieldEffects_ProcedureImmediate_ReturnsExpected (int type) {
        Procedure.Effect[] effects = [new ((Procedure.Effect.EffectType) type, [0])];
        ProcedureImmediate procedure = new (0, effects);

        Procedure.EffectBundle? actual = procedure.YieldEffects (0);

        Assert.IsNotNull (actual);
        CollectionAssert.AreEqual (effects, ((Procedure.EffectBundle) actual!).Effects);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Procedure 0")]
    [DataRow ((byte) 1, "Procedure 1")]
    public void ToString_ProcedureImmediate_ReturnsExpected (byte id, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureImmediate procedure = new (id, [new (Procedure.Effect.EffectType.ElectionParty, [])]);

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (7)]
    [DataRow (8)]
    [DataRow (9)]
    [DataRow (10)]
    [DataRow (11)]
    [DataRow (12)]
    [DataRow (13)]
    public void Constructor_ProcedureTargetedWrongType_Throws (int type) {
        Procedure.Effect effect = new ((Procedure.Effect.EffectType) type, [0]);

        Assert.Throws<ArgumentException> (() => new ProcedureTargeted (0, [effect], []));
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (1)]
    [DataRow (2)]
    [DataRow (3)]
    [DataRow (4)]
    [DataRow (5)]
    [DataRow (6)]
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void YieldEffects_ProcedureTargetedEvery_ReturnsExpected (int type) {
        Procedure.Effect[] effects = [new ((Procedure.Effect.EffectType) type, [0])];
        ProcedureTargeted procedure = new (0, effects, []);

        Procedure.EffectBundle? actual = procedure.YieldEffects (0);

        Assert.IsNotNull (actual);
        CollectionAssert.AreEqual (effects, ((Procedure.EffectBundle) actual!).Effects);
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (1)]
    [DataRow (2)]
    [DataRow (3)]
    [DataRow (4)]
    [DataRow (5)]
    [DataRow (6)]
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void YieldEffects_ProcedureTargetedTarget_ReturnsExpected (int type) {
        Procedure.Effect[] effects = [new ((Procedure.Effect.EffectType) type, [0])];
        ProcedureTargeted procedure = new (0, effects, [0]);

        Procedure.EffectBundle? actual = procedure.YieldEffects (0);

        Assert.IsNotNull (actual);
        CollectionAssert.AreEqual (effects, ((Procedure.EffectBundle) actual!).Effects);
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (1)]
    [DataRow (2)]
    [DataRow (3)]
    [DataRow (4)]
    [DataRow (5)]
    [DataRow (6)]
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void YieldEffects_ProcedureTargetedNoTarget_ReturnsNull (int type) {
        Procedure.Effect[] effects = [new ((Procedure.Effect.EffectType) type, [0])];
        ProcedureTargeted procedure = new (0, effects, [0]);

        Procedure.EffectBundle? actual = procedure.YieldEffects (1);

        Assert.IsNull (actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Procedure 0")]
    [DataRow ((byte) 1, "Procedure 1")]
    public void ToString_ProcedureTargeted_ReturnsExpected (byte id, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureTargeted procedure = new (id, [new (Procedure.Effect.EffectType.CurrencyInitialise, [])], []);

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Every Ballot:")]
    [DataRow (new byte[] { 0 }, "0:")]
    [DataRow (new byte[] { 0, 1 }, "0, 1:")]
    public void ToString_ProcedureTargetedFilter_ReturnsExpected (byte[] filterIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureTargeted procedure = new (0, [new (Procedure.Effect.EffectType.VotePassAdd, [], 1)], [.. filterIds]);

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Every Ballot:")]
    [DataRow (new byte[] { 0 }, "0:")]
    [DataRow (new byte[] { 0, 1 }, "0, 1:")]
    public void ToString_ProcedureTargetedNoFilter_ReturnsExpected (byte[] filterIds, string unexpected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureTargeted procedure = new (0, [new (Procedure.Effect.EffectType.CurrencyInitialise, [])], [.. filterIds]);

        string actual = procedure.ToString (simulation, in localisation);

        Assert.DoesNotContain (unexpected, actual);
    }

    [TestMethod]
    public void Constructor_ProcedureDeclaredBallotPassBallotFail_Throws () {
        Procedure.Effect pass = new (Procedure.Effect.EffectType.BallotPass, []);
        Procedure.Effect fail = new (Procedure.Effect.EffectType.BallotFail, []);

        Assert.Throws<ArgumentException> (() => new ProcedureDeclared (0, [pass, fail], new Confirmation (), []));
        Assert.Throws<ArgumentException> (() => new ProcedureDeclared (0, [fail, pass], new Confirmation (), []));
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (1)]
    [DataRow (2)]
    [DataRow (5)]
    [DataRow (6)]
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void Constructor_ProcedureDeclaredWrongType_Throws (int type) {
        Procedure.Effect effect = new ((Procedure.Effect.EffectType) type, [0]);

        Assert.Throws<ArgumentException> (() => new ProcedureDeclared (0, [effect], new Confirmation (), []));
    }

    [TestMethod]
    [DataRow (3)]
    [DataRow (4)]
    [DataRow (7)]
    [DataRow (8)]
    [DataRow (9)]
    [DataRow (10)]
    [DataRow (11)]
    [DataRow (12)]
    [DataRow (13)]
    public void YieldEffects_ProcedureDeclared_ReturnsExpected (int type) {
        Procedure.Effect[] effects = [new ((Procedure.Effect.EffectType) type, [0])];
        ProcedureDeclared procedure = new (0, effects, new Confirmation (), []);

        Procedure.EffectBundle? actual = procedure.YieldEffects (0);

        Assert.IsNotNull (actual);
        CollectionAssert.AreEqual (effects, ((Procedure.EffectBundle) actual!).Effects);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Procedure 0")]
    [DataRow ((byte) 1, "Procedure 1")]
    public void ToString_ProcedureDeclared_ReturnsExpected (byte id, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureDeclared procedure = new (id, [new (Procedure.Effect.EffectType.BallotPass, [])], new Confirmation (), []);
        string declare = "Can declare if:";

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (declare, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Everyone:")]
    [DataRow (new byte[] { 255 }, "Members:")]
    [DataRow (new byte[] { 255, 254 }, "Members, Government Heads:")]
    public void ToString_ProcedureDeclaredDeclarer_ReturnsExpected (byte[] declarerIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureDeclared procedure = new (0, [new (Procedure.Effect.EffectType.BallotPass, [])], new Confirmation (), [.. declarerIds]);

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (0, "Always")]
    [DataRow (1, "Division of chamber")]
    [DataRow (2, "Can spend 1 Currency")]
    [DataRow (3, "Dice roll greater than or equal to 1")]
    [DataRow (4, "Can spend dice roll Currency")]
    public void ToString_ProcedureDeclaredConfirmation_ReturnsExpected (int type, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Confirmation confirmation = new ((Confirmation.ConfirmationType) type, 1);
        ProcedureDeclared procedure = new (0, [new (Procedure.Effect.EffectType.BallotPass, [])], confirmation, [Role.MEMBER]);
        string declare = "Can declare if:";

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (declare, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (2, "Can spend 1 Currency")]
    [DataRow (4, "Can spend dice roll Currency")]
    [DataRow (5, "Can spend declarer's dice roll Currency")]
    public void ToString_ProcedureDeclaredConfirmationCurrency_ReturnsExpected (int type, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Confirmation confirmation = new ((Confirmation.ConfirmationType) type, 1);
        ProcedureDeclared procedure = new (
            0,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            confirmation,
            [Role.MEMBER, Role.LEADER_PARTY, Role.LEADER_REGION, 0]
        );

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
        Assert.Contains ("Currency State", actual);
        Assert.Contains ("Currency Region", actual);
        Assert.Contains ("Currency Party", actual);
        Assert.Contains ("Currency 0", actual);
    }

    [TestMethod]
    public void ToString_ProcedureDeclaredConfirmationNoCurrency_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        localisation.Currencies.Clear ();
        Confirmation confirmation = new (Confirmation.ConfirmationType.DiceAdversarial);
        ProcedureDeclared procedure = new (
            0,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            confirmation,
            [Role.MEMBER, Role.LEADER_PARTY, Role.LEADER_REGION, 0]
        );
        string declare = "Declarer's dice roll greater than or equal to defender's dice roll";
        string unexpected = "Can spend declarer's dice roll Currency";

        string actual = procedure.ToString (simulation, in localisation);

        Assert.Contains (declare, actual);
        Assert.DoesNotContain (unexpected, actual);
        Assert.DoesNotContain ("Currency State", actual);
        Assert.DoesNotContain ("Currency Region", actual);
        Assert.DoesNotContain ("Currency Party", actual);
        Assert.DoesNotContain ("Currency 0", actual);
    }
}
