using System.Threading.Tasks;
using System.Windows.Input;

public interface IAsyncCommand : IAsyncCommand<object>
{
}

public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
{
    ICommand Command { get; }
    Task ExecuteAsync(T obj);
    bool CanExecute(object obj);
}