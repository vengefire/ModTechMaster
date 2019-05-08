using ModTechMaster.Core.Enums;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Data.Models
{
    public class ObjectReference<TType> : IObjectReference<TType> where TType : IReferenceableObject
    {
        public ObjectReference(TType referenceObject, ObjectReferenceType objectReferenceType,
            IRelationship relationship, bool isActive)
        {
            ReferenceObject = referenceObject;
            ObjectReferenceType = objectReferenceType;
            Relationship = relationship;
            IsActive = isActive;
        }

        public ObjectReferenceType ObjectReferenceType { get; }

        public IRelationship Relationship { get; }

        public TType ReferenceObject { get; }

        public bool IsActive { get; set; }

        public bool IsValid { get; set; }
    }
}