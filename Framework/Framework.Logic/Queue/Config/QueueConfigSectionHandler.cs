using System.Configuration;
using Framework.Logic.Queue.Config.MessageLogicHandler;
using Framework.Logic.Queue.Config.MessageType;
using Framework.Logic.Queue.Config.Queue;
using Framework.Logic.Queue.Config.QueueProcessor;

namespace Framework.Logic.Queue.Config
{
    public class QueueConfigSectionHandler : ConfigurationSection
    {
        public static bool ConfigurationPresent
        {
            get { return ConfigurationManager.GetSection("queues") != null; }
        }

        [ConfigurationProperty("messageTypes", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageTypeCollection))]
        public MessageTypeCollection MessageTypes
        {
            get { return (MessageTypeCollection) this["messageTypes"]; }
        }

        [ConfigurationProperty("messageQueues", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageQueueCollection))]
        public MessageQueueCollection MessageQueues
        {
            get { return (MessageQueueCollection) this["messageQueues"]; }
        }

        [ConfigurationProperty("messageLogicHandlers", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageLogicHandlerCollection))]
        public MessageLogicHandlerCollection MessageLogicHandlers
        {
            get { return (MessageLogicHandlerCollection) this["messageLogicHandlers"]; }
        }

        [ConfigurationProperty("queueProcessors", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(QueueProcessorCollection))]
        public QueueProcessorCollection QueueProcessors
        {
            get { return (QueueProcessorCollection) this["queueProcessors"]; }
        }
    }
}