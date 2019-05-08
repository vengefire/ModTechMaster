using System.Collections.Generic;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IReferenceableObjectProvider
    {
        List<IReferenceableObject> GetReferenceableObjects();
    }
}