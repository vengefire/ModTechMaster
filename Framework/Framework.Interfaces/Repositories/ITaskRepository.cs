namespace Framework.Interfaces.Repositories
{
    using System;

    public interface ITaskRepository
    {
        DateTime? GetLastExecution(string name);

        void UpdateTaskExecution(string name, DateTime executed, TimeSpan duration);
    }
}