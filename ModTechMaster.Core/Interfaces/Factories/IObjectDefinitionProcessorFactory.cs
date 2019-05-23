namespace ModTechMaster.Core.Interfaces.Factories
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Processors;

    public interface IObjectDefinitionProcessorFactory
    {
        IObjectDefinitionProcessor Get(ObjectType type);
    }
}