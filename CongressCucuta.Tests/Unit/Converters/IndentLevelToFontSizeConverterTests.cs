using System.Windows;
using CongressCucuta.Converters;

namespace CongressCucuta.Tests.Unit.Converters;

[TestClass]
public sealed class IndentLevelToFontSizeConverterTests {
    [TestMethod]
    [DataRow ((byte) 0, "28")]
    [DataRow ((byte) 1, "24")]
    [DataRow ((byte) 2, "20")]
    [DataRow ((byte) 3, "16")]
    [DataRow ((byte) 4, "16")]
    public void Convert_Normal_ReturnsExpected (byte value, string expected) {
        IndentLevelToFontSizeConverter converter = new ();

        string result = (string) converter.Convert (value, typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, result);
    }

    [TestMethod]
    public void Convert_WrongValue_ReturnsUnset () {
        IndentLevelToFontSizeConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.Convert ("", typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void ConvertBack_Normal_ReturnsUnset () {
        IndentLevelToFontSizeConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.ConvertBack (new object (), typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }
}
