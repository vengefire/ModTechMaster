using System;

namespace Framework.Domain.Tasks
{
    public enum TaskSchedulerType
    {
        Interval,

        Daily,

        Yearly,

        Monthly
    }

    public static class TaskSchedulerTypeHelper
    {
        public static TaskSchedulerType FromString(string src)
        {
            if (src == "Interval" || src == "IntervalTaskScheduler")
            {
                return TaskSchedulerType.Interval;
            }
            if (src == "Daily" || src == "EveryDayTaskScheduler")
            {
                return TaskSchedulerType.Daily;
            }
            if (src == "Monthly" || src == "EveryMonthTaskScheduler")
            {
                return TaskSchedulerType.Monthly;
            }
            if (src == "Yearly" || src == "DateEveryYearTaskScheduler")
            {
                return TaskSchedulerType.Yearly;
            }
            throw new InvalidProgramException(
                string.Format("Source value [{0}] is not valid for conversion to TaskSchedulerType.", src));
        }
    }
}