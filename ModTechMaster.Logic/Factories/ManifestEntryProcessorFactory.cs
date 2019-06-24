namespace ModTechMaster.Logic.Factories
{
    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Factories;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Logic.Processors;

    public class ManifestEntryProcessorFactory : IManifestEntryProcessorFactory
    {
        public IManifestEntryProcessor Get(ObjectType type, ILogger logger)
        {
            switch (type)
            {
                case ObjectType.SimGameConversations:
                case ObjectType.Texture2D:
                case ObjectType.Sprite:
                    return new ResourceManifestEntryProcessor(logger);
                case ObjectType.AssetBundle:
                    return new AssetBundleManifestEntryProcessor(logger);
                case ObjectType.Prefab:
                    return new PrefabManifestEntryProcessor(logger);
                case ObjectType.CCDefaults:
                case ObjectType.CCCategories:
                    return new CustomComponentsManifestEntryProcessor(logger);
                default:
                    return new ManifestEntryProcessor(logger);
            }
        }
    }
}