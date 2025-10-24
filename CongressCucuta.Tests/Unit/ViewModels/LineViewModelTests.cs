using CongressCucuta.Core;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class LineViewModelTests {
    [TestMethod]
    public void SetContent_True_MutatesExpected () {
        byte indentLevel = 1;
        LineViewModel line = StringLineFormatter.Indent ("Line", indentLevel);

        line.IsContent = true;

        Assert.IsTrue (line.IsContent);
        Assert.AreEqual (indentLevel, line.IndentLevel);
    }

    [TestMethod]
    public void SetContent_False_MutatesExpected () {
        LineViewModel line = StringLineFormatter.Indent ("Line", 1);

        byte expected = 2;
        line.IsContent = false;

        Assert.IsFalse (line.IsContent);
        Assert.AreEqual (expected, line.IndentLevel);
    }

    [TestMethod]
    public void SetSubtitle_True_MutatesExpected () {
        LineViewModel line = StringLineFormatter.Indent ("Line", 1);

        byte expected = 2;
        line.IsSubtitle = true;

        Assert.IsTrue (line.IsSubtitle);
        Assert.AreEqual (expected, line.IndentLevel);
    }

    [TestMethod]
    public void SetSubtitle_False_MutatesExpected () {
        byte indentLevel = 1;
        LineViewModel line = StringLineFormatter.Indent ("Line", indentLevel);

        line.IsSubtitle = false;

        Assert.IsFalse (line.IsSubtitle);
        Assert.AreEqual (indentLevel, line.IndentLevel);
    }
}
