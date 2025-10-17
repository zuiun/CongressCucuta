namespace CongressCucuta.ViewModels;

internal class NameViewModel (string name) : ViewModel {
    private string _name = name;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
}
