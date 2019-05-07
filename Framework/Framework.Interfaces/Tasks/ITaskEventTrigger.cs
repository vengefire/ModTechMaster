using System;

namespace Framework.Interfaces.Tasks
{
    public interface ITaskEventTrigger<TEventArgs>
    {
        event Action<TEventArgs> TriggerEventHandler;

        void StartMonitoring();

        void StopMonitoring();
    }
}