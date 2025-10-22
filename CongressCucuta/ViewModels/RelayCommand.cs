using System.Windows.Input;

namespace CongressCucuta.ViewModels;

// Used for commands that take no parameters
internal class RelayCommand (Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand {
    private readonly Func<object?, bool>? _canExecute = canExecute;
    private readonly Action<object?> _execute = execute;

    public event EventHandler? CanExecuteChanged {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute (object? parameter) => _canExecute is null || _canExecute (parameter);

    public void Execute (object? parameter) => _execute (parameter);
}

// Used for commands that take a parameter
internal class RelayCommand<T> (Action<T> execute, Func<T, bool>? canExecute = null) : ICommand {
    private readonly Func<T, bool>? _canExecute = canExecute;
    private readonly Action<T> _execute = execute;

    public event EventHandler? CanExecuteChanged {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute (object? parameter) {
        if (_canExecute is null) {
            return true;
        } else if (parameter is T p) {
            return _canExecute (p);
        } else if (parameter is null) {
            return false;
        } else {
            throw new NotSupportedException ($"CanExecute can only take a {nameof (T)} parameter");
        }
    }

    public void Execute (object? parameter) {
        if (parameter is T p) {
            _execute (p);
        } else if (parameter is null) {
            throw new ArgumentNullException (nameof (parameter), "Execute must take a parameter");
        } else {
            throw new NotSupportedException ($"Execute can only take a {nameof (T)} parameter");
        }
    }
}
