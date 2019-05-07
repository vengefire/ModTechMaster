using System.Collections.Generic;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IObjectDefinition : IJsonObjectBase, ISourcedFromFile
    {
        IObjectDefinitionDescription ObjectDescription { get; }
        Dictionary<string, object> MetaData { get; }
        string GetId { get; }
    }
}