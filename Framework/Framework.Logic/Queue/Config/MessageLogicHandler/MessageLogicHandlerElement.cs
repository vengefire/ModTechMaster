using System;
using System.ComponentModel;
using System.Configuration;

namespace Framework.Logic.Queue.Config.MessageLogicHandler
{
    /// <summary>
    ///     A simple class for parsing a named servie out of a configuration file.
    /// </summary>
    public class MessageLogicHandlerElement : ConfigurationElement
    {
        /// <summary>
        ///     The name of the queue.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("messageType", IsRequired = true)]
        public string MessageType
        {
            get { return (string) this["messageType"]; }
            set { this["messageType"] = value; }
        }

        [TypeConverter(typeof(TypeNameConverter))]
        [ConfigurationProperty("logicType", IsRequired = true)]
        public Type Type
        {
            get { return this["logicType"] as Type; }
            set { this["logicType"] = value; }
        }
    }
}