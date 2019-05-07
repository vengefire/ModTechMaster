namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectDefinitionDescription
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        string Icon { get; }
    }
}