using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class ElectionContextTests {
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
    [DataRow ((byte) 0, false)]
    [DataRow ((byte) 1, true)]
    public void Constructor_Value_ConstructsExpected (byte value, bool expected) {
        ElectionContext election = new (0, new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [], value));

        bool actual = election.IsRandom;

        Assert.AreEqual (expected, actual);
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
}
