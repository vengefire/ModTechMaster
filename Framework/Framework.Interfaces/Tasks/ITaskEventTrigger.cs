namespace Framework.Interfaces.Tasks
{
    using System;

    public interface ITaskEventTrigger<TEventArgs>
    {
        event Action<TEventArgs> TriggerEventHandler;

        void StartMonitoring();

        void StopMonitoring();
    }
}