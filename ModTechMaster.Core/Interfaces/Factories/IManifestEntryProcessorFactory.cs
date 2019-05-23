namespace ModTechMaster.Core.Interfaces.Factories
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Processors;

    public interface IManifestEntryProcessorFactory
    {
        IManifestEntryProcessor Get(ObjectType type);
    }
}