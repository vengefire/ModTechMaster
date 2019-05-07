namespace ModTechMaster.Logic.Factories
{
    using Core.Enums.Mods;
    using Core.Interfaces.Factories;
    using Core.Interfaces.Processors;
    using Processors;

    public class ManifestEntryProcessorFactory : IManifestEntryProcessorFactory
    {
        public IManifestEntryProcessor Get(ManifestEntryType type)
        {
            switch (type)
            {
                case ManifestEntryType.SimGameConversations:
                case ManifestEntryType.ItemCollectionDef:
                case ManifestEntryType.Texture2D:
                case ManifestEntryType.Sprite:
                    return new ResourceManifestEntryProcessor();
                case ManifestEntryType.AssetBundle:
                    return new AssetBundleManifestEntryProcessor();
                case ManifestEntryType.Prefab:
                    return new PrefabManifestEntryProcessor();
                default:
                    return new ManifestEntryProcessor();
            }
        }
    }
}