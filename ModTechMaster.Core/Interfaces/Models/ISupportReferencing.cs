using System.Collections.Generic;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface ISupportReferencing
    {
        Dictionary<string, object> MetaData { get; }
        void AddMetaData();
        string GetId { get; }
        // bool IsDependency(IRelationship relationship, ISupportReferencing testingObject);
    }
}