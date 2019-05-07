using System.Configuration;
using Framework.Logic.Tasks.Config.Background.Task;
using Framework.Logic.Tasks.Config.Background.Trigger;
using Framework.Logic.Tasks.Config.Scheduled;

namespace Framework.Logic.Tasks.Config
{
    public class TaskConfigSectionHandler : ConfigurationSection
    {
        public static bool ConfigurationPresent
        {
            get { return ConfigurationManager.GetSection("tasks") != null; }
        }

        [ConfigurationProperty("scheduledTasks", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduledTaskCollection))]
        public ScheduledTaskCollection ScheduledTasks
        {
            get { return (ScheduledTaskCollection) this["scheduledTasks"]; }
        }

        [ConfigurationProperty("triggers", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TaskTriggerCollection))]
        public TaskTriggerCollection TaskTriggers
        {
            get { return (TaskTriggerCollection) this["triggers"]; }
        }

        [ConfigurationProperty("backgroundTasks", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(BackgroundTaskCollection))]
        public BackgroundTaskCollection BackgroundTasks
        {
            get { return (BackgroundTaskCollection) this["backgroundTasks"]; }
        }
    }
}