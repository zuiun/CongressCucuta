using CongressCucuta.Core;

namespace CongressCucuta.Tests;

[TestClass]
public sealed class StringLineFormatterTests {
    [TestMethod]
    [DataRow ("Line", (byte) 0, "|")]
    [DataRow ("Line", (byte) 1, "#|")]
    [DataRow ("Line", (byte) 2, "##|")]
    public void Indent_Normal_ReturnsExpected (string line, byte indentLevel, string expected) {
        string actual = StringLineFormatter.Indent (line, indentLevel);

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ("Text", (byte) 0, "Text")]
    [DataRow ("|Text", (byte) 0, "Text")]
    [DataRow ("#|Text", (byte) 1, "Text")]
    [DataRow ("##|Text", (byte) 2, "Text")]
    public void Split_Normal_ReturnsExpected (string text, byte indentLevel, string line) {
        (string, byte) actual = StringLineFormatter.Split (text);

        Assert.AreEqual (line, actual.Item1);
        Assert.AreEqual (indentLevel, actual.Item2);
    }

    [TestMethod]
    [DataRow ("||Text")]
    [DataRow ("#||Text")]
    [DataRow ("#|#|Text")]
    [DataRow ("#|||Text")]
    public void Split_WrongFormat_Throws (string text) {
        Assert.Throws<ArgumentException> (() => StringLineFormatter.Split (text));
    }

    [TestMethod]
    [DataRow ("Text", "Text")]
    [DataRow ("|Text", "Text")]
    [DataRow ("#|Text", "    Text")]
    [DataRow ("##|Text", "        Text")]
    [DataRow ("Text\nText", "Text\nText")]
    [DataRow ("Text\n|Text", "Text\nText")]
    [DataRow ("Text\n#|Text", "Text\n    Text")]
    [DataRow ("Text\n##|Text", "Text\n        Text")]
    public void Convert_Normal_ReturnsExpected (string text, string expected) {
        string actual = StringLineFormatter.Convert (text);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ("Text", "Text")]
    [DataRow ("|Text", "|Text")]
    [DataRow ("#|Text", "|Text")]
    [DataRow ("##|Text", "#|Text")]
    [DataRow ("Text\nText", "Text\nText")]
    [DataRow ("Text\n|Text", "Text\n|Text")]
    [DataRow ("Text\n#|Text", "Text\n|Text")]
    [DataRow ("Text\n##|Text", "Text\n#|Text")]
    public void Outdent_Normal_ReturnsExpected (string text, string expected) {
        string actual = StringLineFormatter.Outdent (text);

        Assert.AreEqual (expected, actual);
    }
}
