namespace ModTechMaster.UI.Core.Async
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /*public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
    {
        public AwaitableDelegateCommand(Func<Task> executeMethod)
            : base(o => executeMethod())
        {
        }

        public AwaitableDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }
    }*/

    public class AwaitableDelegateCommand<T> : IAsyncCommand<T>, ICommand
    {
        private readonly Func<T, Task> executeMethod;

        private readonly DelegateCommand<T> underlyingCommand;

        private bool isExecuting;

        public AwaitableDelegateCommand(Func<T, Task> executeMethod)
            : this(executeMethod, _ => true)
        {
        }

        public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
        }

        public event EventHandler CanExecuteChanged
        {
            add => this.underlyingCommand.CanExecuteChanged += value;
            remove => this.underlyingCommand.CanExecuteChanged -= value;
        }

        public ICommand Command => this;

        public NotifyTaskCompletion Execution { get; set; }

        public bool CanExecute(object parameter)
        {
            return !this.isExecuting && this.underlyingCommand.CanExecute((T)parameter);
        }

        public async void Execute(object parameter)
        {
            await this.ExecuteAsync((T)parameter).ConfigureAwait(false);
        }

        public async Task ExecuteAsync(T obj)
        {
            try
            {
                this.isExecuting = true;
                this.RaiseCanExecuteChanged();

                // await this._executeMethod(obj);
                this.Execution = new NotifyTaskCompletion(this.executeMethod(obj));
                await this.Execution.TaskCompletion;
            }
            finally
            {
                this.isExecuting = false;
                this.RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            this.underlyingCommand.RaiseCanExecuteChanged();
        }
    }
}