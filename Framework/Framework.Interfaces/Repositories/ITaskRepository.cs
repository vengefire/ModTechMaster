using System;

namespace Framework.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        DateTime? GetLastExecution(string name);

        void UpdateTaskExecution(string name, DateTime executed, TimeSpan duration);
    }
}