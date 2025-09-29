namespace congress_cucuta.ViewModels;

internal class LineViewModel : ViewModel {
    private string _text = "Text";
    private byte _indentLevel = 0;
    private bool _isImportant = false;
    private string? _description = null;

    public string Text {
        get => this._text;
        set {
            this._text = value;
            OnPropertyChanged ();
        }
    }

    public byte IndentLevel {
        get => this._indentLevel;
        set {
            this._indentLevel = value;
            OnPropertyChanged ();
        }
    }

    public bool IsImportant {
        get => this._isImportant;
        set {
            this._isImportant = value;
            OnPropertyChanged ();
        }
    }

    public string? Description {
        get => this._description;
        set {
            this._description = value;
            OnPropertyChanged ();
        }
    }
}
