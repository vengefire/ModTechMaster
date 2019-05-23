namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IReferenceableObject : IObject
    {
        Dictionary<string, object> MetaData { get; }

        void AddMetaData();
    }
}