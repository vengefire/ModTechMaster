namespace ModTechMaster.Data.Models
{
    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Models;

    public class ObjectReference<TType> : IObjectReference<TType>
        where TType : class, IReferenceableObject
    {
        public ObjectReference(
            TType referenceObject,
            ObjectReferenceType objectReferenceType,
            IRelationship relationship,
            bool isActive,
            bool isValid,
            string referenceKey)
        {
            this.ReferenceObject = referenceObject;
            this.ObjectReferenceType = objectReferenceType;
            this.Relationship = relationship;
            this.IsActive = isActive;
            this.IsValid = isValid;
            this.ReferenceKey = referenceKey;
        }

        public bool IsActive { get; set; }

        public bool IsValid { get; set; }

        public ObjectReferenceType ObjectReferenceType { get; }

        public string ReferenceKey { get; }

        public TType ReferenceObject { get; }

        public IRelationship Relationship { get; }
    }
}