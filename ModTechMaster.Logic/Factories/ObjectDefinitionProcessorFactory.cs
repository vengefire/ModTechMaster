namespace ModTechMaster.Logic.Factories
{
    using Core.Enums.Mods;
    using Core.Interfaces.Factories;
    using Core.Interfaces.Processors;
    using Processors;

    internal class ObjectDefinitionProcessorFactory : IObjectDefinitionProcessorFactory
    {
        private static ObjectDefinitionProcessorFactory objectDefinitionProcessorFactory;

        public static ObjectDefinitionProcessorFactory ObjectDefinitionProcessorFactorySingleton
        {
            get
            {
                if (ObjectDefinitionProcessorFactory.objectDefinitionProcessorFactory == null)
                {
                    ObjectDefinitionProcessorFactory.objectDefinitionProcessorFactory = new ObjectDefinitionProcessorFactory();
                }

                return ObjectDefinitionProcessorFactory.objectDefinitionProcessorFactory;
            }
        }

        public IObjectDefinitionProcessor Get(ManifestEntryType type)
        {
            switch (type)
            {
                case ManifestEntryType.FactionDef:
                    return new FactionObjectDefinitionProcessor();
                default:
                    return new ObjectDefinitionProcessor();
            }
        }
    }
}