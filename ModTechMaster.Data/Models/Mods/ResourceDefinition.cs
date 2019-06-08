namespace ModTechMaster.Data.Models.Mods
{
    using System;
    using System.Collections.Generic;

    using ModTechMaster.Core.Constants;
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public class ResourceDefinition : SourcedFromFile, IResourceDefinition
    {
        public ResourceDefinition(ObjectType objectType, string sourceFilePath, string name, string id)
            : base(sourceFilePath)
        {
            this.ObjectType = objectType;
            this.Name = name;
            this.Id = id;
        }

        public string Id { get; }

        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

        public Dictionary<string, List<string>> Tags => throw new NotImplementedException();

        public string Name { get; }

        public ObjectType ObjectType { get; }

        public void AddMetaData()
        {
            this.MetaData.Add(Keywords.Id, this.Id);
            this.MetaData.Add(Keywords.Name, this.Name);
        }
    }
}