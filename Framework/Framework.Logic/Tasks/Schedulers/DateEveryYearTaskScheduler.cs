using System;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Schedulers
{
    public class DateEveryYearTaskScheduler : ITaskScheduler
    {
        public DateEveryYearTaskScheduler(int month, int day)
        {
            Month = month;
            Day = day;
        }

        public int Day { get; }

        public int Month { get; }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var runBefore = new DateTime(lastRun.Year, lastRun.Month, lastRun.Day).AddYears(1);

            missed = (lastRun != DateTime.MinValue) && (now > runBefore);
            return (lastRun.Date != now.Date) && (Month == now.Date.Month) && (Day == now.Date.Day);
        }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var nextRun = new DateTime(now.Year, Month, Day);

            if (lastRun > nextRun)
            {
                nextRun.AddYears(1);
            }

            return now.Date == nextRun.Date ? now : nextRun;
        }
    }
}