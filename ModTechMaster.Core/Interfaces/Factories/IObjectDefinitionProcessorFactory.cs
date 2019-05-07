namespace ModTechMaster.Core.Interfaces.Factories
{
    using Enums.Mods;
    using Processors;

    public interface IObjectDefinitionProcessorFactory
    {
        IObjectDefinitionProcessor Get(ManifestEntryType type);
    }
}