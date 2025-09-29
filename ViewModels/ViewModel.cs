using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace congress_cucuta.ViewModels;

public class ViewModel {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged ([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
    }
}
