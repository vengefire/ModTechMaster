using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectDefinition : IJsonObjectBase, ISourcedFromFile, IReferenceableObject, IObject
    {
        IObjectDefinitionDescription ObjectDescription { get; }
    }
}