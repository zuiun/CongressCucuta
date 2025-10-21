using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Tests.Unit.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class LinkTests {
    private readonly record struct Target (IDType ID) : IID;

    [TestMethod]
    public void Resolve_True_ReturnsID () {
        FakeSimulationContext context = new ();
        IDType id = 255;
        Link<Target> link = new (new AlwaysCondition (), id);

        IDType? actual = link.Resolve (context);

        Assert.AreEqual (id, actual);
    }

    [TestMethod]
    public void Resolve_False_ReturnsNull () {
        FakeSimulationContext context = new ();
        IDType id = 255;
        Link<Target> link = new (new NeverCondition (), id);

        IDType? actual = link.Resolve (context);

        Assert.IsNull (actual);
    }
}
