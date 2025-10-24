using CongressCucuta.ViewModels;
using System.Windows;

namespace CongressCucuta.Views;

internal interface IWindow<T, U>
where T : Window
where U : ViewModel {
    T? Window { get; }
    U? DataContext { get; }

    public void New (U? dataContext = null, Action? setup = null);
    public bool? ShowDialog ();
}
