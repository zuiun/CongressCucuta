using CongressCucuta.ViewModels;
using System.Windows;

namespace CongressCucuta.Views;

internal class ModalWindow<T, U> (Func<U?, T> generator) : IWindow<T, U>
where T : Window
where U : ViewModel {
    private T? _window = null;
    private U? _dataContext = null;
    private readonly Func<U?, T> _generator = generator;
    public T? Window => _window;
    public U? DataContext => _dataContext;

    public void New (U? viewModel = null, Action? setup = null) {
        _window = _generator (viewModel);
        _dataContext = viewModel;
        setup?.Invoke ();
    }

    public bool? ShowDialog () => _window?.ShowDialog ();
}
