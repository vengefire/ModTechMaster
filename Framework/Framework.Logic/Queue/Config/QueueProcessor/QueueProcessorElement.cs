using System.Configuration;
using Framework.Domain.Queue;

namespace Framework.Logic.Queue.Config.QueueProcessor
{
    /// <summary>
    ///     A simple class for parsing a named servie out of a configuration file.
    /// </summary>
    public class QueueProcessorElement : ConfigurationElement
    {
        /// <summary>
        ///     The name of the queue.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("messageProcessingMode", IsRequired = true)]
        public MessageProcessingMode MessageProcessingMode
        {
            get { return Convert.ToMessageProcessingMode(this["messageProcessingMode"]); }
            set { this["messageProcessingMode"] = value; }
        }

        [ConfigurationProperty("numWorkers", IsRequired = true)]
        public int NumWorkers
        {
            get { return System.Convert.ToInt32(this["numWorkers"]); }
            set { this["numWorkers"] = value; }
        }

        [ConfigurationProperty("messageQueue", IsRequired = true)]
        public string MessageQueue
        {
            get { return (string) this["messageQueue"]; }
            set { this["messageQueue"] = value; }
        }

        [ConfigurationProperty("messageLogicHandler", IsRequired = true)]
        public string MessageLogicHandler
        {
            get { return (string) this["messageLogicHandler"]; }
            set { this["logicType"] = value; }
        }
    }
}