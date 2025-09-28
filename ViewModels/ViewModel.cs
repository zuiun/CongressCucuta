using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace congress_cucuta.ViewModels;

public class ViewModel : UserControl {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged ([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
    }
}
