using System.Windows;
using CongressCucuta.Converters;

namespace CongressCucuta.Tests.Unit.Converters;

[TestClass]
public sealed class CountToVisibilityConverterTests {
    [TestMethod]
    [DataRow (0, "Collapsed")]
    [DataRow (1, "Visible")]
    public void Convert_Normal_ReturnsExpected (int value, string expected) {
        CountToVisibilityConverter converter = new ();

        string actual = (string) converter.Convert (value, typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Convert_WrongValue_ReturnsUnset () {
        CountToVisibilityConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.Convert ("", typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void ConvertBack_Normal_ReturnsUnset () {
        CountToVisibilityConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.ConvertBack (new object (), typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }
}
