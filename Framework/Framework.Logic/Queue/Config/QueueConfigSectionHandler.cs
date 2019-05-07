namespace Framework.Logic.Queue.Config
{
    using System.Configuration;
    using MessageLogicHandler;
    using MessageType;
    using Queue;
    using QueueProcessor;

    public class QueueConfigSectionHandler : ConfigurationSection
    {
        public static bool ConfigurationPresent => ConfigurationManager.GetSection("queues") != null;

        [ConfigurationProperty("messageTypes", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageTypeCollection))]
        public MessageTypeCollection MessageTypes => (MessageTypeCollection)this["messageTypes"];

        [ConfigurationProperty("messageQueues", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageQueueCollection))]
        public MessageQueueCollection MessageQueues => (MessageQueueCollection)this["messageQueues"];

        [ConfigurationProperty("messageLogicHandlers", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MessageLogicHandlerCollection))]
        public MessageLogicHandlerCollection MessageLogicHandlers => (MessageLogicHandlerCollection)this["messageLogicHandlers"];

        [ConfigurationProperty("queueProcessors", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(QueueProcessorCollection))]
        public QueueProcessorCollection QueueProcessors => (QueueProcessorCollection)this["queueProcessors"];
    }
}