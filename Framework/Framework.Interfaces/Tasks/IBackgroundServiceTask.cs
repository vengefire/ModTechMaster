namespace Framework.Interfaces.Tasks
{
    public interface IBackgroundServiceTask<TEventArgs>
    {
        void ExecuteTask(TEventArgs eventArgs);
    }
}