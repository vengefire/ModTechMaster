using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
{
    public AwaitableDelegateCommand(Func<Task> executeMethod)
        : base(o => executeMethod())
    {
    }

    public AwaitableDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        : base(o => executeMethod(), o => canExecuteMethod())
    {
    }
}

public class AwaitableDelegateCommand<T> : IAsyncCommand<T>, ICommand
{
    private readonly Func<T, Task> _executeMethod;
    private readonly DelegateCommand<T> _underlyingCommand;
    private bool _isExecuting;

    public AwaitableDelegateCommand(Func<T, Task> executeMethod)
        : this(executeMethod, _ => true)
    {
    }

    public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
    {
        this._executeMethod = executeMethod;
        this._underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
    }

    public NotifyTaskCompletion Execution { get; set; }

    public async Task ExecuteAsync(T obj)
    {
        try
        {
            this._isExecuting = true;
            this.RaiseCanExecuteChanged();
            //await this._executeMethod(obj);
            this.Execution = new NotifyTaskCompletion(this._executeMethod(obj));
            await this.Execution.TaskCompletion;
        }
        finally
        {
            this._isExecuting = false;
            this.RaiseCanExecuteChanged();
        }
    }

    public ICommand Command => this;

    public bool CanExecute(object parameter)
    {
        return !this._isExecuting && this._underlyingCommand.CanExecute((T)parameter);
    }

    public void RaiseCanExecuteChanged()
    {
        this._underlyingCommand.RaiseCanExecuteChanged();
    }

    public event EventHandler CanExecuteChanged { add => this._underlyingCommand.CanExecuteChanged += value; remove => this._underlyingCommand.CanExecuteChanged -= value; }

    public async void Execute(object parameter)
    {
        await this.ExecuteAsync((T)parameter);
    }
}