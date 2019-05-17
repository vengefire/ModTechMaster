using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Constants;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ObjectDefinition : JsonObjectSourcedFromFile, IObjectDefinition
    {
        public ObjectDefinition(ObjectType objectType, IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath) : base(objectType, filePath, (JObject)jsonObject)
        {
            this.ObjectDescription = objectDescription;
            this.MetaData = new Dictionary<string, dynamic>();
        }

        public IObjectDefinitionDescription ObjectDescription { get; }
        public Dictionary<string, dynamic> MetaData { get; }

        public virtual void AddMetaData()
        {
            if (this.ObjectDescription?.Id != null)
            {
                this.MetaData.Add(Keywords.Id, this.ObjectDescription?.Id);
            }
            else if (this.JsonObject?.Id != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject.Id);
            }
            else if (this.JsonObject?.identifier != null)
            {
                this.MetaData.Add(Keywords.Id, this.JsonObject?.identifier);
            }

            if (this.ObjectDescription?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.ObjectDescription?.Name);
            }
            else if (this.JsonObject?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.JsonObject.Name);
            }

            // If we didn't find a Name in our json store (which may not be there if we're a resource object) then add our default name (FileName)
            if (!MetaData.ContainsKey(Keywords.Name))
            {
                MetaData.Add(Keywords.Name, Name);
            }
        }

        public override string Name => this.MetaData.ContainsKey(Keywords.Name) ? this.MetaData[Keywords.Name] : this.SourceFileName;
        public override string Id => this.MetaData.ContainsKey(Keywords.Id) ? this.MetaData[Keywords.Id] : this.Name;
    }
}