using CongressCucuta.Core;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class ProcedureEffectTests {
    [TestMethod]
    [DataRow (6)]
    [DataRow (9)]
    [DataRow (10)]
    public void Constructor_NoTarget_Throws (int type) {
        Assert.Throws<ArgumentException> (() => new Procedure.Effect ((Procedure.Effect.EffectType) type, []));
    }

    [TestMethod]
    public void ToString_VotePassAdd_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.VotePassAdd, [], 1);

        string expected = "Gains 1 vote(s) in favour";
        string actual = effect.ToString (simulation, in localisation);
    
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_VoteFailAdd_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.VoteFailAdd, [], 1);

        string expected = "Gains 1 vote(s) in opposition";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_VotePassTwoThirds_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.VotePassTwoThirds, [], 1);

        string expected = "Needs a two-thirds majority to pass";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_CurrencyAddTarget_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.CurrencyAdd, [0, 1, 2, 3], 1);
        string target = "Region 0, Region 1, 2, Party 3";

        string expected = "Gains 1 Currency";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (target, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_CurrencyAddNoTarget_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.CurrencyAdd, [], 1);

        string expected = "Gain 1 Currency State";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_CurrencySubtractTarget_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.CurrencySubtract, [0, 1, 2, 3], 1);
        string target = "Region 0, Region 1, 2, Party 3";

        string expected = "Loses 1 Currency";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (target, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_CurrencySubtractNoTarget_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.CurrencySubtract, [], 1);

        string expected = "Lose 1 Currency State";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_CurrencyInitialiseEvery_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.CurrencyInitialise, []);
        string regions = "Every Region:";
        string parties = "Every Party:";

        string expected = "begins at 1";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (regions, actual);
        Assert.Contains (parties, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_CurrencyInitialiseSingle_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        simulation.CurrenciesValues[0] = 2;
        simulation.CurrenciesValues[2] = 2;
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.CurrencyInitialise, []);
        string region0 = "Region 0:";
        string region1 = "Region 1:";
        string party2 = "2:";
        string party3 = "Party 3:";
        string value1 = "begins at 1";
        string value2 = "begins at 2";

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (region0, actual);
        Assert.Contains (region1, actual);
        Assert.Contains (party2, actual);
        Assert.Contains (party3, actual);
        Assert.Contains (value1, actual);
        Assert.Contains (value2, actual);
    }

    [TestMethod]
    public void ToString_ProcedureActivate_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ProcedureActivate, [0]);

        string expected = "Hold new Procedure 0";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Everyone:")]
    [DataRow (new byte[] { 255 }, "Everyone except Members:")]
    public void ToString_ElectionRegionTarget_ReturnsExpected (byte[] targets, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionRegion, [.. targets]);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Elects Region Leader")]
    [DataRow ((byte) 1, "Randomly appoints Region Leader")]
    public void ToString_ElectionRegionLeader_ReturnsExpected (byte value, string expected) {
        Simulation simulation = new FakeSimulation ();
        simulation.RolesPermissions[Role.LEADER_REGION] = new ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionRegion, [], value);
        string regions = "Every Region:";

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (regions, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_ElectionRegionNoLeader_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionRegion, []);

        string expected = "Randomly aligns with a Region";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Everyone:")]
    [DataRow (new byte[] { 255 }, "Everyone except Members:")]
    public void ToString_ElectionPartyTarget_ReturnsExpected (byte[] targets, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionParty, [.. targets]);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Elects Party Leader")]
    [DataRow ((byte) 1, "Randomly appoints Party Leader")]
    public void ToString_ElectionPartyLeader_ReturnsExpected (byte value, string expected) {
        Simulation simulation = new FakeSimulation ();
        simulation.RolesPermissions[Role.LEADER_PARTY] = new ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionParty, [], value);
        string regions = "Every Party:";

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (regions, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_ElectionPartyNoLeader_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionParty, []);

        string expected = "Randomly aligns with a Party";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Everyone:")]
    [DataRow (new byte[] { 255 }, "Everyone except Members:")]
    [DataRow (new byte[] { 255, 254 }, "Everyone except Members, Government Heads:")]
    public void ToString_ElectionNominated_ReturnsExpected (byte[] candidateIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionNominated, [253, .. candidateIds]);
        string target = "Elect State Head:";
        string nominate = "Can be nominated";

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (target, actual);
        Assert.Contains (expected, actual);
        Assert.Contains (nominate, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Everyone:")]
    [DataRow (new byte[] { 255 }, "Everyone except Members:")]
    [DataRow (new byte[] { 255, 254 }, "Everyone except Members, Government Heads:")]
    public void ToString_ElectionAppointedTarget_ReturnsExpected (byte[] candidateIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionAppointed, [253, .. candidateIds]);
        string target = "Appoint State Head:";
        string appoint = "Can be appointed";

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (target, actual);
        Assert.Contains (expected, actual);
        Assert.Contains (appoint, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Appoint State Head:")]
    [DataRow ((byte) 1, "Randomly appoint State Head:")]
    public void ToString_ElectionAppointedValue_ReturnsExpected (byte value, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.ElectionAppointed, [253, 255], value);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Everyone except Declarer:")]
    [DataRow (new byte[] { 255 }, "Everyone except Declarer, Members:")]
    [DataRow (new byte[] { 255, 254 }, "Everyone except Declarer, Members, Government Heads:")]
    public void ToString_BallotLimit_ReturnsExpected (byte[] targetIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.BallotLimit, [.. targetIds]);
        string ballot = "Current ballot:";
        string vote = "Cannot vote";

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (ballot, actual);
        Assert.Contains (expected, actual);
        Assert.Contains (vote, actual);
    }

    [TestMethod]
    public void ToString_BallotPass_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.BallotPass, []);
        string ballot = "Current ballot:";

        string expected = "Immediately passes";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (ballot, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_BallotFail_ReturnsExpected () {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.BallotFail, []);
        string ballot = "Current ballot:";

        string expected = "Immediately fails";
        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (ballot, actual);
        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Random person:")]
    [DataRow (new byte[] { 255 }, "Members:")]
    [DataRow (new byte[] { 255, 254 }, "Members, Government Heads:")]
    public void ToString_PermissionsCanVoteTarget_ReturnsExpected (byte[] targetIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.PermissionsCanVote, [.. targetIds]);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Cannot vote")]
    [DataRow ((byte) 1, "Can vote")]
    public void ToString_PermissionsCanVoteValue_ReturnsExpected (byte value, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.PermissionsCanVote, [255], value);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Random person:")]
    [DataRow (new byte[] { 255 }, "Members:")]
    [DataRow (new byte[] { 255, 254 }, "Members, Government Heads:")]
    public void ToString_PermissionsVotesTarget_ReturnsExpected (byte[] targetIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.PermissionsVotes, [.. targetIds]);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 1, "Gain 1 vote(s)")]
    [DataRow ((byte) 2, "Gain 2 vote(s)")]
    public void ToString_PermissionsVotesValue_ReturnsExpected (byte value, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.PermissionsVotes, [255], value);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow (new byte[] { }, "Random person:")]
    [DataRow (new byte[] { 255 }, "Members:")]
    [DataRow (new byte[] { 255, 254 }, "Members, Government Heads:")]
    public void ToString_PermissionsCanSpeakTarget_ReturnsExpected (byte[] targetIds, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.PermissionsCanSpeak, [.. targetIds]);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, "Cannot speak")]
    [DataRow ((byte) 1, "Can speak")]
    public void ToString_PermissionsCanSpeakValue_ReturnsExpected (byte value, string expected) {
        Simulation simulation = new FakeSimulation ();
        Localisation localisation = FakeLocalisation.Create ();
        Procedure.Effect effect = new (Procedure.Effect.EffectType.PermissionsCanSpeak, [255], value);

        string actual = effect.ToString (simulation, in localisation);

        Assert.Contains (expected, actual);
    }
}
