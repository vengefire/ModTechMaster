namespace ModTechMaster.Core.Interfaces.Services
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Interfaces.Models;

    public interface IReferenceFinderService
    {
        long ProcessModCollectionReferences(IModCollection modCollection);

        IReferenceableObjectProvider ReferenceableObjectProvider { get; set; }

        List<IObjectReference<IReferenceableObject>> GetObjectReferences(
            IReferenceableObject referenceableObject);

        long ProcessAllReferences();
    }
}