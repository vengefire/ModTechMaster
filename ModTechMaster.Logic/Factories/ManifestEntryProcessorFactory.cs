namespace ModTechMaster.Logic.Factories
{
    using Core.Enums.Mods;
    using Core.Interfaces.Factories;
    using Core.Interfaces.Processors;
    using Processors;

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
                default:
                    return new ManifestEntryProcessor();
            }
        }
    }
}