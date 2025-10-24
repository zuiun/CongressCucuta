using System.Windows;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Fakes;

[TestClass]
public sealed class FakeTests {
    [TestMethod]
    public void Choose_Generator_ReturnsExpected () {
        FakeGenerator generator = new ([0, 1]);

        Assert.AreEqual (0, generator.Choose (100));
        Assert.AreEqual (1, generator.Choose (100));
        Assert.AreEqual (0, generator.Choose (100));
        Assert.AreEqual (1, generator.Choose (100));
    }

    [TestMethod]
    public void Roll_Generator_ReturnsExpected () {
        FakeGenerator generator = new ([0, 1]);

        Assert.AreEqual (0, generator.Roll ());
        Assert.AreEqual (1, generator.Roll ());
        Assert.AreEqual (0, generator.Roll ());
        Assert.AreEqual (1, generator.Roll ());
    }

    [TestMethod]
    public void New_Window_MutatesExpected () {
        FakeWindow<Window, ViewModel> window = new ();

        window.New ();

        Assert.IsNull (window.Window);
        Assert.IsNull (window.DataContext);
    }

    [TestMethod]
    public void ShowDialog_Window_ReturnsExpected () {
        bool isBlock = false;
        FakeWindow<Window, ViewModel> window = new ((_) => isBlock = true);
        window.New (new ());

        bool? actual = window.ShowDialog ();

        Assert.IsNull (actual);
        Assert.IsTrue (isBlock);
    }
}
