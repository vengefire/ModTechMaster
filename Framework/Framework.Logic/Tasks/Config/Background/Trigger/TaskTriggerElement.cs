using System;
using System.ComponentModel;
using System.Configuration;
using Framework.Domain.Tasks;

namespace Framework.Logic.Tasks.Config.Background.Trigger
{
    public class TaskTriggerElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("triggerType", IsRequired = false)]
        public TaskTriggerType TriggerType
        {
            get { return (TaskTriggerType) this["triggerType"]; }
            set { this["triggerType"] = value; }
        }

        [ConfigurationProperty("watchPath", IsRequired = false)]
        public string WatchPath
        {
            get { return (string) this["watchPath"]; }
            set { this["watchPath"] = value; }
        }

        [ConfigurationProperty("fileFilter", IsRequired = false)]
        public string FileFilter
        {
            get { return (string) this["fileFilter"]; }
            set { this["fileFilter"] = value; }
        }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("target", IsRequired = true)]
        public Type Target
        {
            get { return this["target"] as Type; }
            set { this["target"] = value; }
        }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("eventArgsType", IsRequired = true)]
        public Type EventArgsType
        {
            get { return this["eventArgsType"] as Type; }
            set { this["eventArgsType"] = value; }
        }
    }
}