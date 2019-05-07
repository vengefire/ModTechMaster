namespace Framework.Logic.Tasks.Schedulers
{
    using System;
    using Interfaces.Tasks;

    public class IntervalTaskScheduler : ITaskScheduler
    {
        public IntervalTaskScheduler(TimeSpan interval)
        {
            this.Interval = interval;
        }

        public TimeSpan Interval { get; }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var nextRun = lastRun.Add(this.Interval);
            return now > nextRun ? now : nextRun;
        }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var runBefore = lastRun.Add(this.Interval).Add(this.Interval);

            missed = lastRun != DateTime.MinValue && now >= runBefore;
            return now >= lastRun.Add(this.Interval);
        }
    }
}