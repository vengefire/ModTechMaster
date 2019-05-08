namespace ModTechMaster.Core.Interfaces.Factories
{
    using Enums.Mods;
    using Models;

    public interface IObjectDefinitionFactory
    {
        IObjectDefinition Get(ObjectType entryType, IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath);
    }
}