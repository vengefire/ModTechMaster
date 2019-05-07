using System;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Schedulers
{
    public class IntervalTaskScheduler : ITaskScheduler
    {
        public IntervalTaskScheduler(TimeSpan interval)
        {
            Interval = interval;
        }

        public TimeSpan Interval { get; }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var nextRun = lastRun.Add(Interval);
            return now > nextRun ? now : nextRun;
        }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var runBefore = lastRun.Add(Interval).Add(Interval);

            missed = (lastRun != DateTime.MinValue) && (now >= runBefore);
            return now >= lastRun.Add(Interval);
        }
    }
}