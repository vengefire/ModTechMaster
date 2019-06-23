namespace ModTechMaster.Core.Interfaces.Factories
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;

    public interface IObjectDefinitionFactory
    {
        IObjectDefinition Get(
            ObjectType entryType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath,
            IReferenceFinderService referenceFinderService);
    }
}