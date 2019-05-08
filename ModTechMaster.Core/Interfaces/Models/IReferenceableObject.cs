using System.Collections.Generic;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IReferenceableObject : IObject
    {
        Dictionary<string, object> MetaData { get; }
        void AddMetaData();
        string GetId { get; }
    }
}