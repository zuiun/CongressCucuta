namespace congress_cucuta.ViewModels;

public class LineViewModel : ViewModel {
    private string _text = "Text";
    private byte _indentLevel = 0;
    private bool _isImportant = false;
    private string? _hoverText = null;

    public string Text {
        get { return _text; }
        set {
            _text = value;
            OnPropertyChanged ();
        }
    }

    public byte IndentLevel {
        get { return _indentLevel; }
        set {
            _indentLevel = value;
            OnPropertyChanged ();
        }
    }

    public bool IsImportant {
        get { return _isImportant; }
        set {
            _isImportant = value;
            OnPropertyChanged ();
        }
    }

    public string? HoverText {
        get { return _hoverText; }
        set {
            _hoverText = value;
            OnPropertyChanged ();
        }
    }
}
