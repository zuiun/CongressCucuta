using CongressCucuta.Core;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class IDTests {
    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, 0)]
    [DataRow ((byte) 0, (byte) 1, -1)]
    [DataRow ((byte) 1, (byte) 0, 1)]
    public void CompareTo_IDTypes_ReturnsExpected (byte left, byte right, int expected) {
        IDType l = left;
        IDType r = right;

        int actual = l.CompareTo (r);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, true)]
    [DataRow ((byte) 0, (byte) 1, false)]
    [DataRow ((byte) 1, (byte) 0, false)]
    public void Equals_IDTypeByte_ReturnsExpected (byte left, byte right, bool expected) {
        IDType l = left;

        bool actual = l.Equals (right);

        Assert.AreEqual (expected, actual);
    }
}
