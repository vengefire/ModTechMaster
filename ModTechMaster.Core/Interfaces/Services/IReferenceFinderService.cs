namespace ModTechMaster.Core.Interfaces.Services
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public interface IReferenceFinderService
    {
        IReferenceableObjectProvider ReferenceableObjectProvider { get; set; }

        List<IObjectRelationship> GetDependencyRelationships(ObjectType objectType);

        List<IObjectRelationship> GetDependentRelationships(ObjectType objectType);

        List<IObjectReference<IReferenceableObject>> GetObjectReferences(IReferenceableObject referenceableObject);

        long ProcessAllReferences();

        long ProcessModCollectionReferences(IModCollection modCollection);

        void ProcessModCollectionLanceSlotEligibility(IModCollection modCollection);
    }
}