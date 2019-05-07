using System;
using System.ComponentModel;
using System.Configuration;

namespace Framework.Logic.Tasks.Config.Background.Task
{
    public class BackgroundTaskElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("triggerName", IsRequired = true)]
        public string TaskTriggerName
        {
            get { return (string) this["triggerName"]; }
            set { this["triggerName"] = value; }
        }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("target", IsRequired = true)]
        public Type Target
        {
            get { return this["target"] as Type; }
            set { this["target"] = value; }
        }
    }
}