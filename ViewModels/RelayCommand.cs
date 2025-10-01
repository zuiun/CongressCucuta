using System.Windows.Input;

namespace congress_cucuta.ViewModels;

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
        } else if (parameter is null) {
            return false;
        } else {
            /*
             * If the cast fails, then it throws an InvalidCastException
             * This is desirable, as it allows errors to be found more easily
             */
            return _canExecute ((T) parameter);
        }
    }

    public void Execute (object? parameter) {
        if (parameter is null) {
            throw new ArgumentNullException (nameof (parameter), "RelayCommand<T>.Execute () must take a parameter");
        }

        /*
         * If the cast fails, then it throws an InvalidCastException
         * This is desirable, as it allows errors to be found more easily
         */
        _execute ((T) parameter);
    }
}
