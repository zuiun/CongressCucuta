using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Generators;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class ElectionContextTests {
    private static (Dictionary<IDType, SortedSet<IDType>>, Dictionary<IDType, (IDType?, IDType?)>, HashSet<IDType>, HashSet<IDType>) CreateFakes () {
        Dictionary<IDType, SortedSet<IDType>> peopleRoles = [];
        peopleRoles[0] = [0, 255];
        peopleRoles[1] = [1, 255];
        peopleRoles[2] = [2, 255];
        peopleRoles[3] = [3, 255];
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions = [];
        peopleFactions[0] = (2, 0);
        peopleFactions[1] = (3, 1);
        peopleFactions[2] = (2, 0);
        peopleFactions[3] = (3, 1);
        HashSet<IDType> partiesActive = [2, 3];
        HashSet<IDType> regionsActive = [0, 1];

        return (peopleRoles, peopleFactions, partiesActive, regionsActive);
    }

    [TestMethod]
    public void Constructor_NoFilter_Throws () {
        IDType[] filterIds = [];

        Assert.Throws<ArgumentException> (() => new ElectionContext (ElectionContext.ElectionType.ShuffleRemove, filterIds));
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
    [DataRow (14)]
    [DataRow (15)]
    [DataRow (16)]
    public void Constructor_WrongType_Throws (int type) {
        Procedure.Effect effect = new ((Procedure.Effect.EffectType) type, [0], 1);

        Assert.Throws<NotSupportedException> (() => new ElectionContext (0, effect));
    }

    [TestMethod]
    [DataRow ((sbyte) 0, false)]
    [DataRow ((sbyte) 1, true)]
    public void Constructor_BallotValue_ConstructsExpected (sbyte value, bool expected) {
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleRemove, [0], value);

        bool actual = election.IsRandom;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, false)]
    [DataRow ((byte) 1, true)]
    public void Constructor_ProcedureValue_ConstructsExpected (byte value, bool expected) {
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [], value));

        bool actual = election.IsRandom;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (true, (byte) 252)]
    [DataRow (false, (byte) 0)]
    public void Constructor_BallotIsLeaderNeeded_ConstructsExpected (bool isLeaderNeeded, byte expected) {
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleRemove, [0], isLeaderNeeded: isLeaderNeeded);

        IDType actual = election.TargetID;

        Assert.AreEqual<IDType> (expected, actual);
    }

    [TestMethod]
    [DataRow (true, (byte) 251)]
    [DataRow (false, (byte) 0)]
    public void Constructor_ProcedureRegionIsLeaderNeeded_ConstructsExpected (bool isLeaderNeeded, byte expected) {
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, []), isLeaderNeeded);

        IDType actual = election.TargetID;

        Assert.AreEqual<IDType> (expected, actual);
    }

    [TestMethod]
    [DataRow (true, (byte) 252)]
    [DataRow (false, (byte) 0)]
    public void Constructor_ProcedurePartyIsLeaderNeeded_ConstructsExpected (bool isLeaderNeeded, byte expected) {
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, []), isLeaderNeeded);

        IDType actual = election.TargetID;

        Assert.AreEqual<IDType> (expected, actual);
    }

    [TestMethod]
    [DataRow (9, new byte[] { 0 }, (byte) 0)]
    [DataRow (9, new byte[] { 0, 1 }, (byte) 0)]
    [DataRow (10, new byte[] { 0 }, (byte) 0)]
    [DataRow (10, new byte[] { 0, 1 }, (byte) 0)]
    public void Constructor_Target_ConstructsExpected (int type, byte[] targetIds, byte targetId) {
        ElectionContext election = new (0, new Procedure.Effect ((Procedure.Effect.EffectType) type, [.. targetIds]));

        IDType expected = targetId;
        IDType actual = election.TargetID;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (7, new byte[] { 0 }, new byte[] { 0 })]
    [DataRow (7, new byte[] { 0, 1 }, new byte[] { 0, 1 })]
    [DataRow (8, new byte[] { 0 }, new byte[] { 0 })]
    [DataRow (8, new byte[] { 0, 1 }, new byte[] { 0, 1 })]
    [DataRow (9, new byte[] { 0 }, new byte[] { })]
    [DataRow (9, new byte[] { 0, 1 }, new byte[] { 1 })]
    [DataRow (10, new byte[] { 0 }, new byte[] { })]
    [DataRow (10, new byte[] { 0, 1 }, new byte[] { 1 })]
    public void Constructor_Filter_ConstructsExpected (int type, byte[] targetIds, byte[] filterIds) {
        ElectionContext election = new (0, new Procedure.Effect ((Procedure.Effect.EffectType) type, [.. targetIds]));

        IDType[] expected = [.. filterIds];
        IDType[] actual = election.FilterIDs;

        CollectionAssert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (0, 1, -1)]
    [DataRow (0, 0, 0)]
    [DataRow (1, 0, 1)]
    public void CompareTo_Shuffle_ReturnsExpected (int typeLeft, int typeRight, int expected) {
        ElectionContext left = new ((ElectionContext.ElectionType) typeLeft, [0]);
        ElectionContext right = new ((ElectionContext.ElectionType) typeRight, [0]);

        int actual = left.CompareTo (right);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 1, -1)]
    [DataRow ((byte) 0, (byte) 0, 0)]
    [DataRow ((byte) 1, (byte) 0, 1)]
    public void CompareTo_Procedure_ReturnsExpected (byte procedureLeft, byte procedureRight, int expected) {
        ElectionContext left = new (procedureLeft, new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, [0]));
        ElectionContext right = new (procedureRight, new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, [0]));

        int actual = left.CompareTo (right);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Run_ShuffleRemove_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        partiesActive.Remove (2);
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleRemove, [2]);

        (var pr, var pf, var _) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (2, pr[2]);
        Assert.AreNotEqual (2, pf[0].Item1);
        Assert.AreNotEqual (2, pf[2].Item1);
    }

    [TestMethod]
    public void Run_ShuffleAddRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        partiesActive.Add (4);
        DeterministicGenerator generator = new ([3, 0, 0]);
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleAdd, [4], 1, true, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (4, pr[0]);
        Assert.Contains (4, pr[1]);
        Assert.AreNotEqual (4, pf[0].Item1);
        Assert.AreEqual (4, pf[1].Item1);
        Assert.AreNotEqual (4, pf[2].Item1);
        Assert.AreNotEqual (4, pf[3].Item1);
        Assert.Contains (4, g[4].TargetIDs);
        Assert.Contains (252, g[4].TargetIDs);
        Assert.DoesNotContain (0, g[4].PeopleAreCandidates);
        Assert.IsTrue (g[4].PeopleAreCandidates[1]);
    }

    [TestMethod]
    public void Run_ShuffleAddNotRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        partiesActive.Add (4);
        DeterministicGenerator generator = new ([3, 0]);
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleAdd, [4], 0, true, generator: generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (4, pr[0]);
        Assert.DoesNotContain (4, pr[1]);
        Assert.AreNotEqual (4, pf[0].Item1);
        Assert.AreEqual (4, pf[1].Item1);
        Assert.Contains (4, g[4].TargetIDs);
        Assert.Contains (252, g[4].TargetIDs);
        Assert.DoesNotContain (0, g[4].PeopleAreCandidates);
        Assert.IsTrue (g[4].PeopleAreCandidates[1]);
    }

    [TestMethod]
    public void Run_ShuffleAddForce_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        partiesActive.Add (4);
        DeterministicGenerator generator = new ([3, 3, 1]);
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleAdd, [4], generator: generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (4, pr[0]);
        Assert.DoesNotContain (4, pr[1]);
        Assert.AreNotEqual (4, pf[0].Item1);
        Assert.AreEqual (4, pf[1].Item1);
        Assert.Contains (4, g[4].TargetIDs);
        Assert.Contains (252, g[4].TargetIDs);
        Assert.DoesNotContain (0, g[4].PeopleAreCandidates);
        Assert.IsFalse (g[4].PeopleAreCandidates[1]);
    }

    [TestMethod]
    public void Run_ShuffleAddNoLeader_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        partiesActive.Add (4);
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (ElectionContext.ElectionType.ShuffleAdd, [4], generator: generator);

        (var pr, var _, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (4, pr[0]);
        Assert.DoesNotContain (4, pr[1]);
        Assert.Contains (4, g[4].TargetIDs);
        Assert.Contains (252, g[4].TargetIDs);
        Assert.IsFalse (g[4].PeopleAreCandidates[0]);
        Assert.IsFalse (g[4].PeopleAreCandidates[1]);
    }

    [TestMethod]
    public void Run_RegionRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (0, new (Procedure.Effect.EffectType.ElectionRegion, [], 1), true, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.Contains (0, pr[0]);
        Assert.Contains (1, pr[1]);
        Assert.DoesNotContain (0, pr[2]);
        Assert.DoesNotContain (0, pr[3]);
        Assert.AreEqual (0, pf[0].Item2);
        Assert.AreEqual (1, pf[1].Item2);
        Assert.AreEqual (0, pf[2].Item2);
        Assert.AreEqual (0, pf[3].Item2);
        Assert.Contains (0, g[0].TargetIDs);
        Assert.Contains (251, g[0].TargetIDs);
        Assert.Contains (1, g[1].TargetIDs);
        Assert.Contains (251, g[1].TargetIDs);
        Assert.IsTrue (g[0].PeopleAreCandidates[0]);
        Assert.IsTrue (g[1].PeopleAreCandidates[1]);
        Assert.IsFalse (g[0].PeopleAreCandidates[2]);
        Assert.IsFalse (g[0].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_RegionNotRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (0, new (Procedure.Effect.EffectType.ElectionRegion, [], 0), true, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (0, pr[0]);
        Assert.DoesNotContain (1, pr[1]);
        Assert.DoesNotContain (0, pr[2]);
        Assert.DoesNotContain (0, pr[3]);
        Assert.AreEqual (0, pf[0].Item2);
        Assert.AreEqual (1, pf[1].Item2);
        Assert.AreEqual (0, pf[2].Item2);
        Assert.AreEqual (0, pf[3].Item2);
        Assert.Contains (0, g[0].TargetIDs);
        Assert.Contains (251, g[0].TargetIDs);
        Assert.Contains (1, g[1].TargetIDs);
        Assert.Contains (251, g[1].TargetIDs);
        Assert.IsTrue (g[0].PeopleAreCandidates[0]);
        Assert.IsTrue (g[1].PeopleAreCandidates[1]);
        Assert.IsTrue (g[0].PeopleAreCandidates[2]);
        Assert.IsTrue (g[0].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_RegionNoLeader_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (0, new (Procedure.Effect.EffectType.ElectionRegion, [], 0), false, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (0, pr[0]);
        Assert.DoesNotContain (1, pr[1]);
        Assert.DoesNotContain (0, pr[2]);
        Assert.DoesNotContain (0, pr[3]);
        Assert.AreEqual (0, pf[0].Item2);
        Assert.AreEqual (1, pf[1].Item2);
        Assert.AreEqual (0, pf[2].Item2);
        Assert.AreEqual (0, pf[3].Item2);
        Assert.Contains (0, g[0].TargetIDs);
        Assert.Contains (251, g[0].TargetIDs);
        Assert.Contains (1, g[1].TargetIDs);
        Assert.Contains (251, g[1].TargetIDs);
        Assert.IsFalse (g[0].PeopleAreCandidates[0]);
        Assert.IsFalse (g[1].PeopleAreCandidates[1]);
        Assert.IsFalse (g[0].PeopleAreCandidates[2]);
        Assert.IsFalse (g[0].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_PartyRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (0, new (Procedure.Effect.EffectType.ElectionParty, [], 1), true, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.Contains (2, pr[0]);
        Assert.Contains (3, pr[1]);
        Assert.DoesNotContain (2, pr[2]);
        Assert.DoesNotContain (2, pr[3]);
        Assert.AreEqual (2, pf[0].Item1);
        Assert.AreEqual (3, pf[1].Item1);
        Assert.AreEqual (2, pf[2].Item1);
        Assert.AreEqual (2, pf[3].Item1);
        Assert.Contains (2, g[2].TargetIDs);
        Assert.Contains (252, g[2].TargetIDs);
        Assert.Contains (3, g[3].TargetIDs);
        Assert.Contains (252, g[3].TargetIDs);
        Assert.IsTrue (g[2].PeopleAreCandidates[0]);
        Assert.IsTrue (g[3].PeopleAreCandidates[1]);
        Assert.IsFalse (g[2].PeopleAreCandidates[2]);
        Assert.IsFalse (g[2].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_PartyNotRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (0, new (Procedure.Effect.EffectType.ElectionParty, [], 0), true, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (2, pr[0]);
        Assert.DoesNotContain (3, pr[1]);
        Assert.DoesNotContain (2, pr[2]);
        Assert.DoesNotContain (2, pr[3]);
        Assert.AreEqual (2, pf[0].Item1);
        Assert.AreEqual (3, pf[1].Item1);
        Assert.AreEqual (2, pf[2].Item1);
        Assert.AreEqual (2, pf[3].Item1);
        Assert.Contains (2, g[2].TargetIDs);
        Assert.Contains (252, g[2].TargetIDs);
        Assert.Contains (3, g[3].TargetIDs);
        Assert.Contains (252, g[3].TargetIDs);
        Assert.IsTrue (g[2].PeopleAreCandidates[0]);
        Assert.IsTrue (g[3].PeopleAreCandidates[1]);
        Assert.IsTrue (g[2].PeopleAreCandidates[2]);
        Assert.IsTrue (g[2].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_PartyNoLeader_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([0]);
        ElectionContext election = new (0, new (Procedure.Effect.EffectType.ElectionParty, [], 0), false, generator);

        (var pr, var pf, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (2, pr[0]);
        Assert.DoesNotContain (3, pr[1]);
        Assert.AreEqual (2, pf[0].Item1);
        Assert.AreEqual (3, pf[1].Item1);
        Assert.AreEqual (2, pf[2].Item1);
        Assert.AreEqual (2, pf[3].Item1);
        Assert.Contains (2, g[2].TargetIDs);
        Assert.Contains (252, g[2].TargetIDs);
        Assert.Contains (3, g[3].TargetIDs);
        Assert.Contains (252, g[3].TargetIDs);
        Assert.IsFalse (g[2].PeopleAreCandidates[0]);
        Assert.IsFalse (g[3].PeopleAreCandidates[1]);
        Assert.IsFalse (g[2].PeopleAreCandidates[2]);
        Assert.IsFalse (g[2].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_Nominated_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [0], 0));

        (var pr, var _, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (0, pr[0]);
        Assert.Contains (0, g[255].TargetIDs);
        Assert.IsTrue (g[255].PeopleAreCandidates[0]);
        Assert.IsTrue (g[255].PeopleAreCandidates[1]);
        Assert.IsTrue (g[255].PeopleAreCandidates[2]);
        Assert.IsTrue (g[255].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_AppointedRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        DeterministicGenerator generator = new ([3]);
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [0], 1), generator: generator);

        (var pr, var _, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (0, pr[0]);
        Assert.Contains (0, pr[3]);
        Assert.Contains (0, g[254].TargetIDs);
        Assert.DoesNotContain (0, g[254].PeopleAreCandidates);
        Assert.DoesNotContain (1, g[254].PeopleAreCandidates);
        Assert.DoesNotContain (2, g[254].PeopleAreCandidates);
        Assert.IsFalse (g[254].PeopleAreCandidates[3]);
    }

    [TestMethod]
    public void Run_AppointedNotRandom_ReturnsExpected () {
        (var peopleRoles, var peopleFactions, var partiesActive, var regionsActive) = CreateFakes ();
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [0], 0));

        (var pr, var _, var g) = election.Run (peopleRoles, peopleFactions, partiesActive, regionsActive);

        Assert.DoesNotContain (0, pr[0]);
        Assert.Contains (0, g[254].TargetIDs);
        Assert.IsTrue (g[254].PeopleAreCandidates[0]);
        Assert.IsTrue (g[254].PeopleAreCandidates[1]);
        Assert.IsTrue (g[254].PeopleAreCandidates[2]);
        Assert.IsTrue (g[254].PeopleAreCandidates[3]);
    }
}
