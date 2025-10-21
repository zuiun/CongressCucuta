using CongressCucuta.Converters;

namespace CongressCucuta.Models;

internal class LineModel {
    private const byte SUBTITLE_SIZE = 2;
    private byte _indentLevel = 0;
    private bool _isContent = true;
    
    public string Text { get; }
    public byte IndentLevel => _indentLevel;
    public bool IsImportant { get; }
    public string? Description { get; }
    public bool IsContent {
        get => _isContent;
        set {
            _isContent = value;

            // IndentLevel also controls font size
            if (! _isContent) {
                _indentLevel = SUBTITLE_SIZE;
            }
        }
    }
    
    /*
     * Constructed from a text that consists of #|Text
     * Number of # determines IndentLevel
     * | separates # from Text
     */
    public LineModel (string text, bool isImportant = false, string? description = null, bool isContent = true) {
        (Text, _indentLevel) = StringLineFormatter.Split (text);
        IsImportant = isImportant;
        Description = description;
        IsContent = isContent;
    }

    public static implicit operator LineModel (string text) => new (text);
}
