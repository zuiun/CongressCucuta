using System.Windows.Input;

namespace congress_cucuta.ViewModels;

internal class RelayCommand (Action<object?> execute, Func<bool>? canExecute = null) : ICommand {
    private readonly Action<object?> _execute = execute;
    private readonly Func<bool>? _canExecute = canExecute;

    public event EventHandler? CanExecuteChanged {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute (object? parameter) => _canExecute is null || _canExecute ();

    public void Execute (object? parameter) => _execute (parameter);
}
