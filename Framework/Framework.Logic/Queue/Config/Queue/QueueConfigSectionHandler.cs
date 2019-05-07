using System.Configuration;

namespace Framework.Logic.Queue.Config.Queue
{
    internal class QueueConfigSectionHandler_Defunct : ConfigurationSection
    {
        public static bool ConfigurationPresent
        {
            get { return ConfigurationManager.GetSection("QueueConfiguration") != null; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(MessageQueueCollection))]
        //// [ConfigurationCollection(typeof(NamedQueueCollection), AddItemName = "add")]
        public MessageQueueCollection Queues
        {
            get { return (MessageQueueCollection) this[string.Empty]; }
        }
    }
}