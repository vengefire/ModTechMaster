namespace ModTechMaster.Data.Models
{
    using Core.Enums;
    using Core.Interfaces.Models;

    public class ObjectReference<TType> : IObjectReference<TType> where TType : IReferenceableObject
    {
        public ObjectReference(
            TType referenceObject, ObjectReferenceType objectReferenceType,
            IRelationship relationship, bool isActive)
        {
            this.ReferenceObject = referenceObject;
            this.ObjectReferenceType = objectReferenceType;
            this.Relationship = relationship;
            this.IsActive = isActive;
        }

        public ObjectReferenceType ObjectReferenceType { get; }

        public IRelationship Relationship { get; }

        public TType ReferenceObject { get; }

        public bool IsActive { get; set; }

        public bool IsValid { get; set; }
    }
}