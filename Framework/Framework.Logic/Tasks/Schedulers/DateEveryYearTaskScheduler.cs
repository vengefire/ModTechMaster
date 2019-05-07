namespace Framework.Logic.Tasks.Schedulers
{
    using System;
    using Interfaces.Tasks;

    public class DateEveryYearTaskScheduler : ITaskScheduler
    {
        public DateEveryYearTaskScheduler(int month, int day)
        {
            this.Month = month;
            this.Day = day;
        }

        public int Day { get; }

        public int Month { get; }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var runBefore = new DateTime(lastRun.Year, lastRun.Month, lastRun.Day).AddYears(1);

            missed = lastRun != DateTime.MinValue && now > runBefore;
            return lastRun.Date != now.Date && this.Month == now.Date.Month && this.Day == now.Date.Day;
        }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var nextRun = new DateTime(now.Year, this.Month, this.Day);

            if (lastRun > nextRun)
            {
                nextRun.AddYears(1);
            }

            return now.Date == nextRun.Date ? now : nextRun;
        }
    }
}