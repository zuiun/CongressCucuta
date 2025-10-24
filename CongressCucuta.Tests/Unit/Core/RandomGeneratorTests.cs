using CongressCucuta.Core.Generators;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class RandomGeneratorTests {
    [TestMethod]
    public void Choose_Seed_ReturnsExpected () {
        RandomGenerator generator = new (0);

        int expected = 72;
        int actual = generator.Choose (100);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Choose_NoSeed_ReturnsExpected () {
        RandomGenerator generator = new ();

        int actual = generator.Choose (1);

        Assert.IsGreaterThanOrEqualTo (0, actual);
        Assert.IsLessThan (1, actual);
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (-1)]
    public void Choose_NotPositive_Throws (int maximum) {
        RandomGenerator generator = new ();

        Assert.Throws<ArgumentException> (() => generator.Choose (maximum));
    }

    [TestMethod]
    public void Roll_Seed_ReturnsExpected () {
        RandomGenerator generator = new (0);

        int expected = 5;
        int actual = generator.Roll ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Roll_NoSeed_ReturnsExpected () {
        RandomGenerator generator = new ();

        int actual = generator.Roll ();

        Assert.IsGreaterThanOrEqualTo (1, actual);
        Assert.IsLessThanOrEqualTo (6, actual);
    }
}
