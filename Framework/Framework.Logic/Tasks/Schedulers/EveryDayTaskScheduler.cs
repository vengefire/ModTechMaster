using System;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Schedulers
{
    public class EveryDayTaskScheduler : ITaskScheduler
    {
        public EveryDayTaskScheduler(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        public int Hour { get; }

        public int Minute { get; }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var today = now.Date.AddHours(Hour).AddMinutes(Minute);

            return lastRun < today ? today : today.AddDays(1);
        }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var runBefore = lastRun.Date.AddDays(2);

            missed = (lastRun != DateTime.MinValue) && (now > runBefore);
            return (lastRun.Date != now.Date) && (now >= now.Date.AddHours(Hour).AddMinutes(Minute));
        }
    }
}