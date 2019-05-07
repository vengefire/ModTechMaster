using System;
using Framework.Interfaces.Providers;
using Framework.Interfaces.Tasks;

namespace Framework.Logic.Tasks.Schedulers
{
    public class EveryMonthTaskScheduler : ITaskScheduler
    {
        public enum DayScheduleType
        {
            Straight,

            BusinessDay
        }

        private readonly IBusinessDayProvider businessDayProvider;

        private readonly DayScheduleType dayScheduleType;

        public EveryMonthTaskScheduler(int hour, int minute, DayScheduleType dayScheduleType, int day,
            IBusinessDayProvider businessDayProvider)
        {
            Hour = hour;
            Minute = minute;
            this.dayScheduleType = dayScheduleType;
            Day = day;
            this.businessDayProvider = businessDayProvider;
        }

        public int Hour { get; }

        public int Minute { get; }

        public int Day { get; }

        public DateTime NextRun(DateTime lastRun, DateTime now)
        {
            var lastRunSubsequentDate = lastRun == DateTime.MinValue
                ? lastRun
                : GetOffSetRunDate(new DateTime(lastRun.Year, lastRun.Month, 1).AddMonths(1), Day);
            var thisMonthRunDate = GetOffSetRunDate(new DateTime(now.Year, now.Month, 1), Day);
            return thisMonthRunDate > lastRunSubsequentDate ? thisMonthRunDate : lastRunSubsequentDate;
        }

        public bool RunTask(DateTime lastRun, DateTime now, out bool missed)
        {
            var lastRunSubsequentDate = lastRun == DateTime.MinValue
                ? lastRun
                : GetOffSetRunDate(new DateTime(lastRun.Year, lastRun.Month, 1).AddMonths(1), Day);
            var thisMonthRunDate = GetOffSetRunDate(new DateTime(now.Year, now.Month, 1), Day);
            var nextRunDate = thisMonthRunDate > lastRunSubsequentDate ? thisMonthRunDate : lastRunSubsequentDate;
            missed = (lastRun != DateTime.MinValue) && (lastRun.Date < thisMonthRunDate) && (now > thisMonthRunDate);
            return (lastRun.Date != now.Date) && (now >= nextRunDate);
        }

        public static DayScheduleType DayScheduleTypeFromString(string src)
        {
            if (src == "Straight")
            {
                return DayScheduleType.Straight;
            }
            if (src == "BusinessDay")
            {
                return DayScheduleType.BusinessDay;
            }
            throw new InvalidProgramException(string.Format("Value [{0}] is not parsable to DayScheduleType", src));
        }

        private DateTime GetOffSetRunDate(DateTime date, int days)
        {
            var offSetDate = date;
            switch (dayScheduleType)
            {
                case DayScheduleType.Straight:
                    offSetDate = date.AddDays(days);
                    break;
                case DayScheduleType.BusinessDay:
                    offSetDate = businessDayProvider.AddBusinessDays(date, days);
                    break;
            }

            return offSetDate;
        }
    }
}