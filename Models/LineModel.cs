namespace congress_cucuta.Models;

internal class LineModel {
    public const char INDENT = '#';
    public const char DELIMITER = '.';

    /*
     * Constructed from a text that consists of #.Text
     * Number of # determines IndentLevel
     * . is a delimiter
     */
    public LineModel (string text, bool isImportant = false, string? description = null) {
        string[] parameters = text.Split (DELIMITER);

        if (parameters.Length != 2) {
            throw new ArgumentException ("Expected '#.Text' format for text");
        }

        IndentLevel = (byte) parameters[0].Count (c => c == INDENT);
        Text = parameters[1];
        IsImportant = isImportant;
        Description = description;
    }

    public string Text { get; }
    public byte IndentLevel { get; }
    public bool IsImportant { get; }
    public string? Description { get; }

    public static implicit operator LineModel (string text) {
        return new (text);
    }
}
