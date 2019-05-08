namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IReferenceableObjectProvider
    {
        List<IReferenceableObject> GetReferenceableObjects();
    }
}