using System.Windows;
using CongressCucuta.Converters;

namespace CongressCucuta.Tests.Unit.Converters;

[TestClass]
public sealed class DescriptionToForegroundConverterTests {
    [TestMethod]
    [DataRow ("", "Blue")]
    [DataRow (null, "Black")]
    public void Convert_Normal_ReturnsExpected (string? value, string expected) {
        DescriptionToForegroundConverter converter = new ();

        string actual = (string) converter.Convert (value, typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Convert_WrongValue_ReturnsUnset () {
        DescriptionToForegroundConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.Convert (0, typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void ConvertBack_Normal_ReturnsUnset () {
        DescriptionToForegroundConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.ConvertBack (new object (), typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }
}
