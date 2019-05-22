namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Constants;
    using Core.Enums.Mods;
    using Core.Interfaces.Models;

    public class ResourceDefinition : SourcedFromFile, IResourceDefinition
    {
        public ResourceDefinition(ObjectType objectType, string sourceFilePath, string name, string id) : base(sourceFilePath)
        {
            this.ObjectType = objectType;
            this.Name = name;
            this.Id = id;
        }

        public ObjectType ObjectType { get; }
        public string Name { get; }
        public string Id { get; }
        public Dictionary<string, object> MetaData { get; } = new Dictionary<string, object>();

        public void AddMetaData()
        {
            this.MetaData.Add(Keywords.Id, this.Id);
            this.MetaData.Add(Keywords.Name, this.Name);
        }
    }
}