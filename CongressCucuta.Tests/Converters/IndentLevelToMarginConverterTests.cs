using System.Windows;
using CongressCucuta.Converters;

namespace CongressCucuta.Tests.Unit.Converters;

[TestClass]
public sealed class IndentLevelToMarginConverterTests {
    [TestMethod]
    [DataRow ((byte) 0, "0")]
    [DataRow ((byte) 1, "24")]
    [DataRow ((byte) 2, "48")]
    [DataRow ((byte) 3, "72")]
    [DataRow ((byte) 4, "96")]
    public void Convert_Normal_ReturnsExpected (byte value, string margin) {
        IndentLevelToMarginConverter converter = new ();

        string expected = $"{margin}, 0, 0, 0";
        string result = (string) converter.Convert (value, typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, result);
    }

    [TestMethod]
    public void Convert_WrongValue_ReturnsUnset () {
        IndentLevelToMarginConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.Convert ("", typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void ConvertBack_Normal_ReturnsUnset () {
        IndentLevelToMarginConverter converter = new ();

        object expected = DependencyProperty.UnsetValue;
        object actual = converter.ConvertBack (new object (), typeof (string), new object (), System.Globalization.CultureInfo.InvariantCulture);

        Assert.AreEqual (expected, actual);
    }
}
