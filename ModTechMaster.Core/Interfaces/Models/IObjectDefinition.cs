namespace ModTechMaster.Core.Interfaces.Models
{
    using ModTechMaster.Core.Enums;

    public interface IObjectDefinition : ISourcedFromFile, IReferenceableObject, IObject
    {
        string HumanReadableText { get; }

        IObjectDefinitionDescription ObjectDescription { get; }

        ObjectStatus ObjectStatus { get; set; }
    }
}