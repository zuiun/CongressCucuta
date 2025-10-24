using System.Windows;
using CongressCucuta.ViewModels;
using CongressCucuta.Views;

namespace CongressCucuta.Tests.Unit.Views;

[TestClass]
public sealed class ModalWindowTests {
    [TestMethod]
    public void New_Normal_MutatesExpected () {
        bool isSetup = false;
#pragma warning disable CS8603 // This generator will never be called
        ModalWindow<Window, ViewModel> window = new ((v) => null);
#pragma warning restore CS8603

        window.New (setup: () => isSetup = true);

        Assert.IsNull (window.Window);
        Assert.IsNull (window.DataContext);
        Assert.IsTrue (isSetup);
    }

    [TestMethod]
    public void ShowDialog_Normal_ReturnsExpected () {
        ModalWindow<Window, ViewModel> window = new ((v) => new Window ());

        bool? actual = window.ShowDialog ();

        Assert.IsNull (actual);
    }
}
