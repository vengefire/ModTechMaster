namespace Framework.Logic.Queue.Config.MessageType
{
    using System;
    using System.ComponentModel;
    using System.Configuration;

    /// <summary>
    ///     A simple class for parsing a named servie out of a configuration file.
    /// </summary>
    public class MessageTypeElement : ConfigurationElement
    {
        /// <summary>
        ///     The name of the queue.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get => (string)this["name"]; set => this["name"] = value; }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("type", IsRequired = true)]
        public Type Type { get => this["type"] as Type; set => this["type"] = value; }
    }
}