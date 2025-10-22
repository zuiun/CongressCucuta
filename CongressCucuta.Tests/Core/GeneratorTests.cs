using CongressCucuta.Core.Generators;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class GeneratorTests {
    [TestMethod]
    public void Choose_RandomSeed_ReturnsExpected () {
        RandomGenerator generator = new (0);

        int expected = 72;
        int actual = generator.Choose (100);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Choose_RandomNoSeed_ReturnsExpected () {
        RandomGenerator generator = new ();

        int actual = generator.Choose (1);

        Assert.IsGreaterThanOrEqualTo (0, actual);
        Assert.IsLessThan (1, actual);
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (-1)]
    public void Choose_RandomNotPositive_Throws (int maximum) {
        RandomGenerator generator = new ();

        Assert.Throws<ArgumentException> (() => generator.Choose (maximum));
    }

    [TestMethod]
    public void Roll_RandomSeed_ReturnsExpected () {
        RandomGenerator generator = new (0);

        int expected = 5;
        int actual = generator.Roll ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Roll_RandomNoSeed_ReturnsExpected () {
        RandomGenerator generator = new ();

        int actual = generator.Roll ();

        Assert.IsGreaterThanOrEqualTo (1, actual);
        Assert.IsLessThanOrEqualTo (6, actual);
    }

    [TestMethod]
    public void Choose_Deterministic_ReturnsExpected () {
        DeterministicGenerator generator = new ([0, 1]);

        Assert.AreEqual (0, generator.Choose (100));
        Assert.AreEqual (1, generator.Choose (100));
        Assert.AreEqual (0, generator.Choose (100));
        Assert.AreEqual (1, generator.Choose (100));
    }

    [TestMethod]
    public void Roll_Deterministic_ReturnsExpected () {
        DeterministicGenerator generator = new ([0, 1]);

        Assert.AreEqual (0, generator.Roll ());
        Assert.AreEqual (1, generator.Roll ());
        Assert.AreEqual (0, generator.Roll ());
        Assert.AreEqual (1, generator.Roll ());
    }
}
