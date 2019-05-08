namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IObjectDefinition : IJsonObjectBase, ISourcedFromFile, ISupportReferencing
    {
        IObjectDefinitionDescription ObjectDescription { get; }
        /*Dictionary<string, object> MetaData { get; }

        string GetId { get; }

        void AddMetaData();*/
    }
}