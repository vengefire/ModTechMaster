namespace ModTechMaster.Core.Interfaces.Models
{
    using ModTechMaster.Core.Enums;

    public interface IObjectReference : IObjectReference<object>
    {
    }

    public interface IObjectReference<TType>
    {
        bool IsActive { get; set; }

        bool IsValid { get; set; }

        ObjectReferenceType ObjectReferenceType { get; }

        string ReferenceKey { get; }

        TType ReferenceObject { get; }

        IRelationship Relationship { get; }
    }
}