namespace ModTechMaster.Core.Interfaces.Factories
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public interface IObjectDefinitionFactory
    {
        IObjectDefinition Get(
            ObjectType entryType,
            IObjectDefinitionDescription objectDescription,
            dynamic jsonObject,
            string filePath);
    }
}