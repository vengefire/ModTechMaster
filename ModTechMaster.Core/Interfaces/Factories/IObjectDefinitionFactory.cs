namespace ModTechMaster.Core.Interfaces.Factories
{
    using Enums.Mods;
    using Models;

    public interface IObjectDefinitionFactory
    {
        IObjectDefinition Get(ManifestEntryType entryType, IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath);
    }
}