namespace Framework.Interfaces.Tasks
{
    using System;

    public interface ITaskScheduler
    {
        /// <summary>
        ///     Determines if a task should be executed.
        /// </summary>
        /// <param name="lastRun">The last run.</param>
        /// <param name="now">The now.</param>
        /// <param name="missed">if set to <c>true</c> a run slot has been missed.</param>
        /// <returns><c>true</c> if the task should be executed, <c>false</c> otherwise.</returns>
        bool RunTask(DateTime lastRun, DateTime now, out bool missed);

        /// <summary>
        ///     Gets the next date/time to execute the task.
        /// </summary>
        /// <param name="lastRun">The last run.</param>
        /// <param name="now">The now.</param>
        /// <returns><see cref="TimeSpan" />.</returns>
        DateTime NextRun(DateTime lastRun, DateTime now);
    }
}