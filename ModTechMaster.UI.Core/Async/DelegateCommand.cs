using System;
using System.Windows.Input;

/*public class DelegateCommand : DelegateCommand<object>
{
    public DelegateCommand(Action executeMethod)
        : base(o => executeMethod())
    {
    }

    public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        : base(o => executeMethod(), o => canExecuteMethod())
    {
    }
}*/

/// <summary>
///     A command that calls the specified delegate when the command is executed.
/// </summary>
/// <typeparam name="T"></typeparam>
public class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged
{
    private readonly Func<T, bool> canExecuteMethod;

    private readonly Action<T> executeMethod;

    private bool isExecuting;

    public DelegateCommand(Action<T> executeMethod)
        : this(executeMethod, null)
    {
    }

    public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
    {
        this.executeMethod = executeMethod ?? throw new ArgumentNullException(
                                  "executeMethod",
                                  @"Execute Method cannot be null");
        this.canExecuteMethod = canExecuteMethod;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(T parameter)
    {
        if (this.canExecuteMethod == null)
        {
            return true;
        }

        return this.canExecuteMethod(parameter);
    }

    public void Execute(T parameter)
    {
        this.executeMethod(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }

    bool ICommand.CanExecute(object parameter)
    {
        return !this.isExecuting && this.CanExecute((T)parameter);
    }

    void ICommand.Execute(object parameter)
    {
        this.isExecuting = true;
        try
        {
            this.RaiseCanExecuteChanged();
            this.Execute((T)parameter);
        }
        finally
        {
            this.isExecuting = false;
            this.RaiseCanExecuteChanged();
        }
    }
}