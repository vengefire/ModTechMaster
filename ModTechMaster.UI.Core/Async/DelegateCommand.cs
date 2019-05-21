using System;
using System.Windows.Input;

public class DelegateCommand : DelegateCommand<object>
{
    public DelegateCommand(Action executeMethod)
        : base(o => executeMethod())
    {
    }

    public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        : base(o => executeMethod(), o => canExecuteMethod())
    {
    }
}

/// <summary>
///     A command that calls the specified delegate when the command is executed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged
{
    private readonly Func<T, bool> _canExecuteMethod;
    private readonly Action<T> _executeMethod;
    private bool _isExecuting;

    public DelegateCommand(Action<T> executeMethod)
        : this(executeMethod, null)
    {
    }

    public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
    {
        this._executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod", @"Execute Method cannot be null");
        this._canExecuteMethod = canExecuteMethod;
    }

    public event EventHandler CanExecuteChanged { add => CommandManager.RequerySuggested += value; remove => CommandManager.RequerySuggested -= value; }

    bool ICommand.CanExecute(object parameter)
    {
        return !this._isExecuting && this.CanExecute((T)parameter);
    }

    void ICommand.Execute(object parameter)
    {
        this._isExecuting = true;
        try
        {
            this.RaiseCanExecuteChanged();
            this.Execute((T)parameter);
        }
        finally
        {
            this._isExecuting = false;
            this.RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }

    public bool CanExecute(T parameter)
    {
        if (this._canExecuteMethod == null)
        {
            return true;
        }

        return this._canExecuteMethod(parameter);
    }

    public void Execute(T parameter)
    {
        this._executeMethod(parameter);
    }
}