namespace Framework.Logic.Tasks.Config
{
    using System.Configuration;
    using Background.Task;
    using Background.Trigger;
    using Scheduled;

    public class TaskConfigSectionHandler : ConfigurationSection
    {
        public static bool ConfigurationPresent => ConfigurationManager.GetSection("tasks") != null;

        [ConfigurationProperty("scheduledTasks", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduledTaskCollection))]
        public ScheduledTaskCollection ScheduledTasks => (ScheduledTaskCollection)this["scheduledTasks"];

        [ConfigurationProperty("triggers", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(TaskTriggerCollection))]
        public TaskTriggerCollection TaskTriggers => (TaskTriggerCollection)this["triggers"];

        [ConfigurationProperty("backgroundTasks", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(BackgroundTaskCollection))]
        public BackgroundTaskCollection BackgroundTasks => (BackgroundTaskCollection)this["backgroundTasks"];
    }
}