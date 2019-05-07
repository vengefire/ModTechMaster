namespace Framework.Logic.Tasks.Async
{
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces.Async;
    using Interfaces.Tasks;

    public class BackgroundTaskRunner<TEventArgs> : IServiceTaskRunner
    {
        private readonly CancellationToken cancellationToken;
        private readonly IBackgroundServiceTask<TEventArgs> serviceTask;

        private readonly ITaskEventTrigger<TEventArgs> taskEventTrigger;

        public BackgroundTaskRunner(
            string name,
            IBackgroundServiceTask<TEventArgs> serviceTask,
            ICancellationTokenProvider cancellationTokenProvider,
            ITaskEventTrigger<TEventArgs> taskEventTrigger)
        {
            this.Name = name;
            this.serviceTask = serviceTask;
            this.cancellationToken = cancellationTokenProvider.CancellationToken;
            this.taskEventTrigger = taskEventTrigger;
            this.taskEventTrigger.TriggerEventHandler += this.serviceTask.ExecuteTask;
        }

        public string Name { get; }

        public Task Task { get; private set; }

        public async Task StartProcessing()
        {
            this.Task = new Task(this.Execute, TaskCreationOptions.LongRunning);
            this.Task.Start();
            await this.Task;
        }

        private void Execute()
        {
            this.taskEventTrigger.StartMonitoring();
            while (!this.cancellationToken.IsCancellationRequested) Thread.Sleep(1);

            this.taskEventTrigger.StopMonitoring();
        }
    }
}