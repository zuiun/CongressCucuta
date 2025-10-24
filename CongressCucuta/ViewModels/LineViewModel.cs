using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

internal class LineViewModel : ViewModel {
    private const byte SUBTITLE_SIZE = 2;
    private bool _isContent;
    private bool _isSubtitle;
    private byte _indentLevel = 0;
    public string Text { get; }
    public byte IndentLevel {
        get => _indentLevel;
        set {
            _indentLevel = value;
            OnPropertyChanged ();
        }
    }
    public bool IsImportant { get; }
    public string? Description { get; }
    public bool IsContent {
        get => _isContent;
        set {
            _isContent = value;
            OnPropertyChanged ();

            // IndentLevel also controls font size
            if (! _isContent) {
                IndentLevel = SUBTITLE_SIZE;
            }
        }
    }
    public bool IsSubtitle {
        get => _isSubtitle;
        set {
            _isSubtitle = value;
            OnPropertyChanged ();

            // IndentLevel also controls font size
            if (_isSubtitle) {
                IndentLevel = SUBTITLE_SIZE;
            }
        }
    }

    public LineViewModel (string text, bool isImportant = false, string? description = null, bool isContent = true) {
        (Text, _indentLevel) = StringLineFormatter.Split (text);
        IsImportant = isImportant;
        Description = description;
        IsContent = isContent;
        IsSubtitle = ! isContent;
    }

    public static implicit operator LineViewModel (string text) => new (text);
}
