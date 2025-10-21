using CongressCucuta.Core;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class FactionTests {
    [TestMethod]
    [DataRow ((byte) 255)]
    [DataRow ((byte) 254)]
    [DataRow ((byte) 253)]
    [DataRow ((byte) 252)]
    [DataRow ((byte) 251)]
    [DataRow ((byte) 250)]
    [DataRow ((byte) 249)]
    [DataRow ((byte) 248)]
    public void Constructor_ReservedRole_Throws (byte id) {
        Assert.Throws<ArgumentException> (() => new Faction (id));
    }
}
