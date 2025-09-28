namespace congress_cucuta.Models;

public class LineModel (string text, byte indentLevel, bool isImportant = false, string? hoverText = null) {
    public string Text { get; } = text;
    public byte IndentLevel { get; } = indentLevel;
    public bool IsImportant { get; } = isImportant;
    public string? HoverText { get; } = hoverText;
}
