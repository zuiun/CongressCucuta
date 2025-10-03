using congress_cucuta.Converters;

namespace congress_cucuta.Models;

internal class LineModel {
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
        (Text, IndentLevel) = StringLineFormatter.Split (text);

        // IndentLevel also controls font size
        if (! isContent) {
            IndentLevel = SUBTITLE_SIZE;
        }

        IsImportant = isImportant;
        Description = description;
        IsContent = isContent;
    }

    public static implicit operator LineModel (string text) => new (text);
}
