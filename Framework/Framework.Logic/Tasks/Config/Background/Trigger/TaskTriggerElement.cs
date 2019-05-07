namespace Framework.Logic.Tasks.Config.Background.Trigger
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using Domain.Tasks;

    public class TaskTriggerElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get => (string)this["name"]; set => this["name"] = value; }

        [ConfigurationProperty("triggerType", IsRequired = false)]
        public TaskTriggerType TriggerType { get => (TaskTriggerType)this["triggerType"]; set => this["triggerType"] = value; }

        [ConfigurationProperty("watchPath", IsRequired = false)]
        public string WatchPath { get => (string)this["watchPath"]; set => this["watchPath"] = value; }

        [ConfigurationProperty("fileFilter", IsRequired = false)]
        public string FileFilter { get => (string)this["fileFilter"]; set => this["fileFilter"] = value; }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("target", IsRequired = true)]
        public Type Target { get => this["target"] as Type; set => this["target"] = value; }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("eventArgsType", IsRequired = true)]
        public Type EventArgsType { get => this["eventArgsType"] as Type; set => this["eventArgsType"] = value; }
    }
}