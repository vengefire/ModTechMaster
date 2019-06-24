namespace ModTechMaster.Core.Interfaces.Factories
{
    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Processors;

    public interface IManifestEntryProcessorFactory
    {
        IManifestEntryProcessor Get(ObjectType type, ILogger logger);
    }
}