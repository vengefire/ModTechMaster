namespace ModTechMaster.Data.Models.Mods
{
    using System.Collections.Generic;
    using Core.Constants;
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ObjectDefinition : JsonObjectSourcedFromFile, IObjectDefinition
    {
        public ObjectDefinition(IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath) : base(filePath, (JObject)jsonObject)
        {
            this.ObjectDescription = objectDescription;
            this.MetaData = new Dictionary<string, dynamic>();
        }

        public IObjectDefinitionDescription ObjectDescription { get; }

        public Dictionary<string, dynamic> MetaData { get; }

        public virtual string GetId => this.ObjectDescription?.Id;
        public virtual void AddMetaData()
        {
            if (this.ObjectDescription?.Id != null)
            {
                this.MetaData.Add(Keywords.Id, this.ObjectDescription?.Id);
            }

            if (this.ObjectDescription?.Name != null)
            {
                this.MetaData.Add(Keywords.Name, this.ObjectDescription?.Name);
            }
        }
    }
}