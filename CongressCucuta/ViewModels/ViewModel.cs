using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class ViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged ([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke (this, new (propertyName));
}
