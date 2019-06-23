namespace ModTechMaster.Core.Interfaces.Models
{
    using ModTechMaster.Core.Enums.Mods;

    public interface IObject
    {
        string Id { get; }

        string Name { get; }

        ObjectType ObjectType { get; }

        IValidationResult ValidateObject();
    }
}