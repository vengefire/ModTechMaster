namespace Framework.Logic.Tasks.Config.Background.Task
{
    using System;
    using System.ComponentModel;
    using System.Configuration;

    public class BackgroundTaskElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get => (string)this["name"]; set => this["name"] = value; }

        [ConfigurationProperty("triggerName", IsRequired = true)]
        public string TaskTriggerName { get => (string)this["triggerName"]; set => this["triggerName"] = value; }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("target", IsRequired = true)]
        public Type Target { get => this["target"] as Type; set => this["target"] = value; }
    }
}