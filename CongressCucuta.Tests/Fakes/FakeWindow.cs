using System.Windows;
using CongressCucuta.ViewModels;
using CongressCucuta.Views;

namespace CongressCucuta.Tests.Fakes;

internal class FakeWindow<T, U> (Action<U>? block = null) : IWindow<T, U>
where T : Window
where U : ViewModel {
    private U? _dataContext = null;
    public T? Window => null;
    public U? DataContext => _dataContext;

    public void New (U? dataContext = null, Action? setup = null) => _dataContext = dataContext;

    public bool? ShowDialog () {
        if (_dataContext is U dataContext) {
            block?.Invoke (dataContext);
        }

        return null;
    }
}
