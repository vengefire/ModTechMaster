namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectDefinition : ISourcedFromFile, IReferenceableObject, IObject
    {
        string HumanReadableText { get; }

        IObjectDefinitionDescription ObjectDescription { get; }
    }
}