namespace Framework.Logic.Tasks.AutoTypeRegisters
{
    using System.Collections.Generic;
    using Async;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Config;
    using Config.Scheduled;
    using Domain.Tasks;
    using Interfaces.Tasks;
    using Schedulers;

    public static class ScheduledTaskAutoRegister
    {
        public static void RegisterConfiguredScheduledTasks(TaskConfigSectionHandler config, IWindsorContainer container)
        {
            var registrations = new List<IRegistration>();

            if (config == null ||
                config.ScheduledTasks == null)
            {
                return;
            }

            foreach (ScheduledTaskElement taskConfig in config.ScheduledTasks)
            {
                registrations.Clear();
                var taskName = taskConfig.Name;
                var scheduleName = string.Empty;
                var schedulerType = TaskSchedulerTypeHelper.FromString(taskConfig.ScheduleType);

                switch (schedulerType)
                {
                    case TaskSchedulerType.Interval:
                        registrations.Add(ScheduledTaskAutoRegister.RegisterNamedIntervalScheduler(taskName, taskConfig, out scheduleName));
                        break;
                    case TaskSchedulerType.Daily:
                        registrations.Add(ScheduledTaskAutoRegister.RegisterNamedDailyScheduler(taskName, taskConfig, out scheduleName));
                        break;
                    case TaskSchedulerType.Monthly:
                        registrations.Add(ScheduledTaskAutoRegister.RegisterNamedMonthlyScheduler(taskName, taskConfig, out scheduleName));
                        break;
                    case TaskSchedulerType.Yearly:
                        registrations.Add(ScheduledTaskAutoRegister.RegisterNamedYearlyScheduler(taskName, taskConfig, out scheduleName));
                        break;
                }

                registrations.AddRange(ScheduledTaskAutoRegister.RegisterNamedScheduledTask(taskName, scheduleName, taskConfig));
                container.Register(registrations.ToArray());
            }
        }

        private static IRegistration[] RegisterNamedScheduledTask(
            string taskName,
            string scheduledName,
            ScheduledTaskElement taskConfig)
        {
            var registrations = new List<IRegistration>();
            var parameters = new Dictionary<string, object> {{"name", taskName}};

            registrations.Add(Component.For<IServiceTask>().ImplementedBy(taskConfig.Target).Named(taskName + "Impl"));
            registrations.Add(
                              Component.For<IServiceTaskRunner>()
                                       .ImplementedBy<ScheduledTaskRunner>()
                                       .DependsOn(parameters)
                                       .DependsOn(Dependency.OnComponent(typeof(ITaskScheduler), scheduledName)).Named(taskName)
                                       .DependsOn(Dependency.OnComponent(typeof(IServiceTask), taskName + "Impl")));

            return registrations.ToArray();
        }

        private static IRegistration RegisterNamedYearlyScheduler(
            string name,
            ScheduledTaskElement taskConfig,
            out string scheduleName)
        {
            var parameters = new Dictionary<string, object> {{"month", taskConfig.Month}, {"day", taskConfig.Day}};

            scheduleName = string.Format("yearlyScheduled-{0}", name);
            return
                Component.For<ITaskScheduler>()
                         .ImplementedBy<DateEveryYearTaskScheduler>()
                         .DependsOn(parameters)
                         .Named(scheduleName);
        }

        private static IRegistration RegisterNamedMonthlyScheduler(
            string name,
            ScheduledTaskElement taskConfig,
            out string scheduleName)
        {
            var parameters = new Dictionary<string, object>
                             {
                                 {"hour", taskConfig.Time.Hour},
                                 {"minute", taskConfig.Time.Minute},
                                 {"day", taskConfig.Day},
                                 {"dayScheduleType", EveryMonthTaskScheduler.DayScheduleTypeFromString(taskConfig.DayScheduleType)}
                             };

            scheduleName = string.Format("monthlyScheduled-{0}", name);
            return
                Component.For<ITaskScheduler>()
                         .ImplementedBy<EveryMonthTaskScheduler>()
                         .DependsOn(parameters)
                         .Named(scheduleName);
        }

        private static IRegistration RegisterNamedDailyScheduler(
            string name,
            ScheduledTaskElement taskConfig,
            out string scheduleName)
        {
            var parameters = new Dictionary<string, object> {{"hour", taskConfig.Time.Hour}, {"minute", taskConfig.Time.Minute}};

            scheduleName = string.Format("dailyScheduled-{0}", name);
            return
                Component.For<ITaskScheduler>()
                         .ImplementedBy<EveryDayTaskScheduler>()
                         .DependsOn(parameters)
                         .Named(scheduleName);
        }

        private static IRegistration RegisterNamedIntervalScheduler(
            string name,
            ScheduledTaskElement taskConfig,
            out string scheduleName)
        {
            var parameters = new Dictionary<string, object> {{"interval", taskConfig.Interval}};

            scheduleName = string.Format("intervalScheduled-{0}", name);
            return
                Component.For<ITaskScheduler>()
                         .ImplementedBy<IntervalTaskScheduler>()
                         .DependsOn(parameters)
                         .Named(scheduleName);
        }
    }
}