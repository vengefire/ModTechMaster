using System.Configuration;

namespace Framework.Logic.Queue.Config.QueueProcessor
{
    public class QueueProcessorConfigSectionHandler : ConfigurationSection
    {
        public static bool ConfigurationPresent
        {
            get { return ConfigurationManager.GetSection("QueueConfiguration") != null; }
        }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(QueueProcessorCollection))]
        //// [ConfigurationCollection(typeof(NamedQueueCollection), AddItemName = "add")]
        public QueueProcessorCollection QueueProcessors
        {
            get { return (QueueProcessorCollection) this[string.Empty]; }
        }
    }
}