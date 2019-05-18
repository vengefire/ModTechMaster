namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectDefinition : ISourcedFromFile, IReferenceableObject, IObject
    {
        IObjectDefinitionDescription ObjectDescription { get; }
        string HumanReadableText { get; }
    }
}