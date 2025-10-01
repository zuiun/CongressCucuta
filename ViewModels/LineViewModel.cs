namespace congress_cucuta.ViewModels;

internal class LineViewModel : ViewModel {
    private string _text = "Text";
    private byte _indentLevel = 0;
    private bool _isImportant = false;
    private string? _description = null;

    public string Text {
        get => _text;
        set {
            _text = value;
            OnPropertyChanged ();
        }
    }

    public byte IndentLevel {
        get => _indentLevel;
        set {
            _indentLevel = value;
            OnPropertyChanged ();
        }
    }

    public bool IsImportant {
        get => _isImportant;
        set {
            _isImportant = value;
            OnPropertyChanged ();
        }
    }

    public string? Description {
        get => _description;
        set {
            _description = value;
            OnPropertyChanged ();
        }
    }
}
