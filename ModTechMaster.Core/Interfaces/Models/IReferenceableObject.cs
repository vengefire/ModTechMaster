namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IReferenceableObject : IObject
    {
        Dictionary<string, object> MetaData { get; }

        Dictionary<string, List<string>> Tags { get; }

        void AddMetaData();
    }
}