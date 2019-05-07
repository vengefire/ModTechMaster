using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Xml;
using Framework.Interfaces.Injection;
using Framework.Interfaces.Logging;
using Framework.Interfaces.Providers;
using Framework.Interfaces.Repositories;
using Framework.Interfaces.Tasks;
using Framework.Logic.Tasks.Schedulers;

namespace Framework.Logic.Tasks.Config
{
    [Obsolete("Use the new TaskConfigSectionHandler instead, with associated Async centric implementation.")]
    public class ServiceTaskConfigSectionHandler : IConfigurationSectionHandler
    {
        private static readonly Type ServiceTaskType = typeof(IServiceTask);

        public ServiceTaskConfigSectionHandler()
            : this(Interfaces.Injection.Container.Instance.GetInstance<IContainer>())
        {
        }

        public ServiceTaskConfigSectionHandler(IContainer container)
        {
            Container = container;
        }

        private IContainer Container { get; }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var taskPersistenceProvider = Container.GetInstance<ITaskRepository>();
            var dateTimeProvider = Container.GetInstance<IDateTimeProvider>();
            var exceptionLogger = Container.GetInstance<IExceptionLogger>();

            var tasks = new List<TaskRunner>();

            foreach (XmlNode child in section.ChildNodes)
            {
                if (child.Name == "task")
                {
                    if (child.Attributes["name"] == null || string.IsNullOrEmpty(child.Attributes["name"].Value))
                    {
                        RaiseException(child, "Attribute '{0}' expected on task element", "name");
                    }

                    if (child.Attributes["target"] == null)
                    {
                        RaiseException(child, "Attribute '{0}' expected on task element", "target");
                    }

                    if (child.Attributes["type"] == null)
                    {
                        RaiseException(child, "Attribute '{0}' expected on task element", "type");
                    }

                    ITaskScheduler taskScheduler;
                    switch (child.Attributes["type"].Value.Trim())
                    {
                        case "IntervalTaskScheduler":
                            taskScheduler = GetPerformPerIntervalTaskType(child);
                            break;
                        case "EveryDayTaskScheduler":
                            taskScheduler = GetPerformEveryDay(child);
                            break;
                        case "DateEveryYearTaskScheduler":
                            taskScheduler = GetPerformDateEveryYear(child);
                            break;
                        case "EveryMonthTaskScheduler":
                            taskScheduler = GetPerformDateEveryMonth(child, Container);
                            break;
                        default:
                            RaiseException(child,
                                "Attribute '{0}' expects a value of 'IntervalTaskScheduler' or 'EveryDayTaskScheduler' or 'DateEveryYearTaskScheduler'",
                                "type");
                            taskScheduler = null;
                            break;
                    }

                    var name = child.Attributes["name"].Value.Trim();

                    if (tasks.Any(x => x.Name.Equals(name)))
                    {
                        RaiseException(child, "{0} is not unique ({1})", "name", name);
                    }

                    var serviceTaskType = GetIServiceTaskType(child, child.Attributes["target"].InnerText);

                    tasks.Add(new TaskRunner(name, serviceTaskType, taskScheduler, Container, taskPersistenceProvider,
                        dateTimeProvider, exceptionLogger));
                }
            }

            return tasks;
        }

        private static Type GetIServiceTaskType(XmlNode node, string target)
        {
            var type = Type.GetType(target);
            if (type == null)
            {
                RaiseException(node, "Cannot find type: '{0}'", target);
            }

            if (!ServiceTaskType.IsAssignableFrom(type))
            {
                RaiseException(node, "'{0}' does not inherit '{1}'", type, ServiceTaskType);
            }

            return type;
        }

        private static ITaskScheduler GetPerformDateEveryYear(XmlNode child)
        {
            if (child.Attributes["month"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "month");
            }

            if (child.Attributes["day"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "day");
            }

            var month = Convert.ToInt32(child.Attributes["month"].Value);
            var day = Convert.ToInt32(child.Attributes["day"].Value);

            return new DateEveryYearTaskScheduler(month, day);
        }

        private static ITaskScheduler GetPerformDateEveryMonth(XmlNode child, IContainer container)
        {
            if (child.Attributes["time"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "time");
            }

            DateTime time;
            var value = child.Attributes["time"].Value;
            if (!DateTime.TryParseExact(value, "H:mm", null, DateTimeStyles.AssumeLocal, out time))
            {
                RaiseException(child, "Unable to parse time '{0}' ({1})", "time", value);
            }

            if (child.Attributes["day"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "day");
            }

            var day = Convert.ToInt32(child.Attributes["day"].Value);

            if (child.Attributes["dayScheduleType"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "dayScheduleType");
            }

            EveryMonthTaskScheduler.DayScheduleType scheduleType;
            if (!Enum.TryParse(child.Attributes["dayScheduleType"].Value, out scheduleType))
            {
                RaiseException(child, "Unable to parse DayScheduleType '{0}' ({1})", "dayScheduleType",
                    child.Attributes["dayScheduleType"].Value);
            }

            return new EveryMonthTaskScheduler(time.Hour, time.Minute, scheduleType, day,
                container.GetInstance<IBusinessDayProvider>());
        }

        private static ITaskScheduler GetPerformEveryDay(XmlNode child)
        {
            if (child.Attributes["time"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "time");
            }

            DateTime time;
            var value = child.Attributes["time"].Value;
            if (!DateTime.TryParseExact(value, "H:mm", null, DateTimeStyles.AssumeLocal, out time))
            {
                RaiseException(child, "Unable to parse time '{0}' ({1})", "time", value);
            }

            return new EveryDayTaskScheduler(time.Hour, time.Minute);
        }

        private static ITaskScheduler GetPerformPerIntervalTaskType(XmlNode child)
        {
            if (child.Attributes["interval"] == null)
            {
                RaiseException(child, "Attribute '{0}' expected on task element", "interval");
            }

            var interval = child.Attributes["interval"].Value;

            TimeSpan timeSpan;
            if (!TimeSpan.TryParse(interval, out timeSpan))
            {
                RaiseException(child, "Attribute '{0}' on task element is not value ({0})", "interval", interval);
            }

            return new IntervalTaskScheduler(timeSpan);
        }

        private static void RaiseException(XmlNode node, string format, params object[] p)
        {
            throw new ConfigurationErrorsException(string.Format(format, p), node);
        }
    }
}