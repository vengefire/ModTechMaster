namespace Framework.Logic.Queue.Config.QueueProcessor
{
    using System.Configuration;

    public class QueueProcessorConfigSectionHandler : ConfigurationSection
    {
        public static bool ConfigurationPresent => ConfigurationManager.GetSection("QueueConfiguration") != null;

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(QueueProcessorCollection))]
        //// [ConfigurationCollection(typeof(NamedQueueCollection), AddItemName = "add")]
        public QueueProcessorCollection QueueProcessors => (QueueProcessorCollection)this[string.Empty];
    }
}