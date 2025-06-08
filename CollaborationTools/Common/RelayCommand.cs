using System.Windows.Input;

namespace CollaborationTools.Common;

public class RelayCommand : ICommand
{
    private readonly Func<object, bool> _canExecute; //실행 가능 여부 확인

    // RelayCommand 구현
    private readonly Action<object> _execute; //실행할 메서드

    public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }
}