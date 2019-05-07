using System.Threading;
using System.Threading.Tasks;
using Framework.Interfaces.Async;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Async
{
    public class BackgroundTaskRunner<TEventArgs> : IServiceTaskRunner
    {
        private readonly CancellationToken cancellationToken;
        private readonly IBackgroundServiceTask<TEventArgs> serviceTask;

        private readonly ITaskEventTrigger<TEventArgs> taskEventTrigger;

        public BackgroundTaskRunner(string name, IBackgroundServiceTask<TEventArgs> serviceTask,
            ICancellationTokenProvider cancellationTokenProvider, ITaskEventTrigger<TEventArgs> taskEventTrigger)
        {
            Name = name;
            this.serviceTask = serviceTask;
            cancellationToken = cancellationTokenProvider.CancellationToken;
            this.taskEventTrigger = taskEventTrigger;
            this.taskEventTrigger.TriggerEventHandler += this.serviceTask.ExecuteTask;
        }

        public string Name { get; }

        public Task Task { get; private set; }

        public async Task StartProcessing()
        {
            Task = new Task(Execute, TaskCreationOptions.LongRunning);
            Task.Start();
            await Task;
        }

        private void Execute()
        {
            taskEventTrigger.StartMonitoring();
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1);
            }

            taskEventTrigger.StopMonitoring();
        }
    }
}