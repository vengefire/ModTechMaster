namespace Framework.Logic.Tasks.Schedulers
{
    using System;
    using Interfaces.Tasks;

    public class EveryDayTaskScheduler : ITaskScheduler
    {
        public EveryDayTaskScheduler(int hour, int minute)
        {
            this.Hour = hour;
            this.Minute = minute;
        }

        public int Hour { get; }

        public int Minute { get; }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var today = now.Date.AddHours(this.Hour).AddMinutes(this.Minute);

            return lastRun < today ? today : today.AddDays(1);
        }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var runBefore = lastRun.Date.AddDays(2);

            missed = lastRun != DateTime.MinValue && now > runBefore;
            return lastRun.Date != now.Date && now >= now.Date.AddHours(this.Hour).AddMinutes(this.Minute);
        }
    }
}