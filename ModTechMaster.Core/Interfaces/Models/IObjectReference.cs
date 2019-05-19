using ModTechMaster.Core.Enums;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectReference : IObjectReference<object>
    {
    }

    public interface IObjectReference<TType>
    {
        TType ReferenceObject { get; }
        ObjectReferenceType ObjectReferenceType { get; }
        IRelationship Relationship { get; }
        bool IsActive { get; set; }
        bool IsValid { get; set; }
    }}