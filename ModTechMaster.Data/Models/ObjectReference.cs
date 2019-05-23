namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Models;

    public class ObjectReference<TType> : IObjectReference<TType>
        where TType : IReferenceableObject
    {
        public ObjectReference(
            TType referenceObject,
            ObjectReferenceType objectReferenceType,
            IRelationship relationship,
            bool isActive)
        {
            this.ReferenceObject = referenceObject;
            this.ObjectReferenceType = objectReferenceType;
            this.Relationship = relationship;
            this.IsActive = isActive;
        }

        public bool IsActive { get; set; }

        public bool IsValid { get; set; }

        public ObjectReferenceType ObjectReferenceType { get; }

        public TType ReferenceObject { get; }

        public IRelationship Relationship { get; }
    }
}