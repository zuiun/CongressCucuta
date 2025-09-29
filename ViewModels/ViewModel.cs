using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace congress_cucuta.ViewModels;

internal class ViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged ([CallerMemberName] string? propertyName = null) {
        this.PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
    }
}
