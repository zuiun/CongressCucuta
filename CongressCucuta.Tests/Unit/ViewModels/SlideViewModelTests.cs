using System.Diagnostics;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class SlideViewModelTests {
    [TestMethod]
    public void Forward_Links_ConstructsExpected () {
        SlideViewModel slide = SlideViewModel.Forward (0, "", []);

        IDType expected = 1;
        IDType actual = slide.Links[0].Link.TargetID;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Backward_Links_ConstructsExpected () {
        SlideViewModel slide = SlideViewModel.Backward (1, "", []);

        IDType expected = 0;
        IDType actual = slide.Links[0].Link.TargetID;

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Bidirectional_Links_ConstructsExpected () {
        SlideViewModel actual = SlideViewModel.Bidirectional (1, "", []);

        Assert.AreEqual<IDType> (0, actual.Links[0].Link.TargetID);
        Assert.AreEqual<IDType> (2, actual.Links[1].Link.TargetID);
    }

    [TestMethod]
    public void Branching_LinksNormal_ConstructsExpected () {
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
    public void Branching_LinksEmpty_Throws () {
        Localisation localisation = FakeLocalisation.Create ();
        
        Assert.Throws<UnreachableException> (() => SlideViewModel.Branching (
            0,
            "Title",
            ["Description"],
            [],
            in localisation
        ));
    }

    [TestMethod]
    public void Constant_Links_ConstructsExpected () {
        SlideViewModel actual = SlideViewModel.Constant (0, "", []);

        Assert.IsEmpty (actual.Links);
    }

    [TestMethod]
    public void FindLink_Correct_ReturnsExpected () {
        SlideViewModel slide = SlideViewModel.Forward (0, "", []);

        IDType expected = 1;
        Link<SlideViewModel> actual = (Link<SlideViewModel>) slide.FindLink (Shortcut.Right)!;

        Assert.AreEqual (expected, actual.TargetID);
    }

    [TestMethod]
    [DataRow (0)]
    [DataRow (1)]
    [DataRow (2)]
    public void FindLink_Other_ReturnsNothing (int shortcut) {
        SlideViewModel slide = SlideViewModel.Forward (0, "", []);

        Link<SlideViewModel>? actual = slide.FindLink ((Shortcut) shortcut);

        Assert.IsNull (actual);
    }
}
