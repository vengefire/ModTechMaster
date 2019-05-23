namespace ModTechMaster.Logic.Factories
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Factories;
    using ModTechMaster.Core.Interfaces.Processors;
    using ModTechMaster.Logic.Processors;

    public class ManifestEntryProcessorFactory : IManifestEntryProcessorFactory
    {
        public IManifestEntryProcessor Get(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.SimGameConversations:
                case ObjectType.Texture2D:
                case ObjectType.Sprite:
                    return new ResourceManifestEntryProcessor();
                case ObjectType.AssetBundle:
                    return new AssetBundleManifestEntryProcessor();
                case ObjectType.Prefab:
                    return new PrefabManifestEntryProcessor();
                case ObjectType.CCDefaults:
                case ObjectType.CCCategories:
                    return new CustomComponentsManifestEntryProcessor();
                default:
                    return new ManifestEntryProcessor();
            }
        }
    }
}