namespace Framework.Logic.Tasks.Config.Scheduled
{
    using System;
    using System.ComponentModel;
    using System.Configuration;

    public class ScheduledTaskElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get => (string)this["name"]; set => this["name"] = value; }

        [ConfigurationProperty("type", IsRequired = true)]
        public string ScheduleType { get => (string)this["type"]; set => this["type"] = value; }

        [ConfigurationProperty("time", IsRequired = true)]
        public DateTime Time { get => Convert.ToDateTime(this["time"]); set => this["time"] = value; }

        [ConfigurationProperty("day", IsRequired = false)]
        public int Day { get => Convert.ToInt32(this["day"]); set => this["day"] = value; }

        [ConfigurationProperty("month", IsRequired = false)]
        public int Month { get => Convert.ToInt32(this["month"]); set => this["month"] = value; }

        [ConfigurationProperty("dayScheduleType", IsRequired = false)]
        public string DayScheduleType { get => (string)this["dayScheduleType"]; set => this["dayScheduleType"] = value; }

        [ConfigurationProperty("interval", IsRequired = false)]
        public TimeSpan Interval { get => TimeSpan.Parse(this["interval"].ToString()); set => this["interval"] = value; }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("target", IsRequired = true)]
        public Type Target { get => this["target"] as Type; set => this["target"] = value; }
    }
}