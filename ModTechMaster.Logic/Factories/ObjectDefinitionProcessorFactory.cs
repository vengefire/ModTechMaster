namespace ModTechMaster.Logic.Factories
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Factories;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Logic.Processors;

    internal class ObjectDefinitionProcessorFactory : IObjectDefinitionProcessorFactory
    {
        private static ObjectDefinitionProcessorFactory objectDefinitionProcessorFactory;

        public static ObjectDefinitionProcessorFactory ObjectDefinitionProcessorFactorySingleton
        {
            get
            {
                if (objectDefinitionProcessorFactory == null)
                {
                    objectDefinitionProcessorFactory = new ObjectDefinitionProcessorFactory();
                }

                return objectDefinitionProcessorFactory;
            }
        }

        public IObjectDefinitionProcessor Get(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.FactionDef:
                    return new FactionObjectDefinitionProcessor();
                case ObjectType.ItemCollectionDef:
                    return new ItemCollectionObjectDefinitionProcessor();
                default:
                    return new ObjectDefinitionProcessor();
            }
        }
    }
}