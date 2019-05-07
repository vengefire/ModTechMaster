using System;
using System.ComponentModel;
using System.Configuration;

namespace Framework.Logic.Tasks.Config.Scheduled
{
    public class ScheduledTaskElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string ScheduleType
        {
            get { return (string) this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("time", IsRequired = true)]
        public DateTime Time
        {
            get { return Convert.ToDateTime(this["time"]); }
            set { this["time"] = value; }
        }

        [ConfigurationProperty("day", IsRequired = false)]
        public int Day
        {
            get { return Convert.ToInt32(this["day"]); }
            set { this["day"] = value; }
        }

        [ConfigurationProperty("month", IsRequired = false)]
        public int Month
        {
            get { return Convert.ToInt32(this["month"]); }
            set { this["month"] = value; }
        }

        [ConfigurationProperty("dayScheduleType", IsRequired = false)]
        public string DayScheduleType
        {
            get { return (string) this["dayScheduleType"]; }
            set { this["dayScheduleType"] = value; }
        }

        [ConfigurationProperty("interval", IsRequired = false)]
        public TimeSpan Interval
        {
            get { return TimeSpan.Parse(this["interval"].ToString()); }
            set { this["interval"] = value; }
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