namespace congress_cucuta.Models;

internal class LineModel {
    public const char INDENT = '#';
    public const char DELIMITER = '.';
    private const byte SUBTITLE_SIZE = 3;
    
    public string Text { get; }
    public byte IndentLevel { get; }
    public bool IsImportant { get; }
    public string? Description { get; }
    public bool IsContent { get; set; }
    
    /*
     * Constructed from a text that consists of #.Text
     * Number of # determines IndentLevel
     * . separates # from Text
     */
    public LineModel (string text, bool isImportant = false, string? description = null, bool isContent = true) {
        string[] parameters = text.Split (DELIMITER);

        if (parameters.Length == 1) {
            // IndentLevel also controls font size
            IndentLevel = isContent ? (byte) 0 : SUBTITLE_SIZE;
            Text = parameters[0];
        } else if (parameters.Length == 2) {
            IndentLevel = (byte) parameters[0].Count (c => c == INDENT);
            Text = parameters[1];
        } else {
            throw new ArgumentException ("Expected '#.Text' format for text");
        }

        IsImportant = isImportant;
        Description = description;
        IsContent = isContent;
    }

    public static implicit operator LineModel (string text) => new (text);
}
