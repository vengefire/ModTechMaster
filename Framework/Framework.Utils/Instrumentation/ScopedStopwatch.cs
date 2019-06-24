namespace Framework.Utils.Instrumentation
{
    using System;
    using System.Diagnostics;

    using Castle.Core.Logging;

    public class ScopedStopwatch : IDisposable
    {
        private readonly ILogger logger;

        private readonly Stopwatch stopwatch = new Stopwatch();

        public ScopedStopwatch(ILogger logger)
        {
            this.logger = logger;
            this.stopwatch.Start();
        }

        private long ElapsedMilliseconds => this.stopwatch.ElapsedMilliseconds;

        public void Dispose()
        {
            this.stopwatch.Stop();
            this.logger.Info($"Operation took [{this.ElapsedMilliseconds}]ms to complete.");
        }
    }
}