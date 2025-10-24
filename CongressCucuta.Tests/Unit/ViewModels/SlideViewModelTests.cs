using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class SlideViewModelTests {
    [TestMethod]
    public void Forward_Links_ConstructsExpected () {
        SlideViewModel slide = SlideViewModel.Forward (0, "Title", ["Description"]);

        IDType expected = 1;
        IDType actual = slide.Links[0].Link.TargetID;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Backward_Links_ConstructsExpected () {
        SlideViewModel slide = SlideViewModel.Backward (1, "Title", ["Description"]);

        IDType expected = 0;
        IDType actual = slide.Links[0].Link.TargetID;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Bidirectional_Links_ConstructsExpected () {
        SlideViewModel actual = SlideViewModel.Bidirectional (1, "Title", ["Description"]);

        Assert.AreEqual<IDType> (0, actual.Links[0].Link.TargetID);
        Assert.AreEqual<IDType> (2, actual.Links[1].Link.TargetID);
    }

    [TestMethod]
    public void Branching_Links_ConstructsExpected () {
        Localisation localisation = FakeLocalisation.Create ();
        SlideViewModel actual = SlideViewModel.Branching (
            0,
            "Title",
            ["Description"],
            [new (new AlwaysCondition (), 1), new (new AlwaysCondition (), 2)],
            in localisation
        );

        Assert.AreEqual<IDType> (1, actual.Links[0].Link.TargetID);
        Assert.AreEqual<IDType> (2, actual.Links[1].Link.TargetID);
    }

    [TestMethod]
    public void Constant_Links_ConstructsExpected () {
        SlideViewModel actual = SlideViewModel.Constant (0, "Title", ["Description"]);

        Assert.IsEmpty (actual.Links);
    }
}
