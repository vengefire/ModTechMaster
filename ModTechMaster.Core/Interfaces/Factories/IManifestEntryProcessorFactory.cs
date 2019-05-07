namespace ModTechMaster.Core.Interfaces.Factories
{
    using Enums.Mods;
    using Processors;

    public interface IManifestEntryProcessorFactory
    {
        IManifestEntryProcessor Get(ManifestEntryType type);
    }
}