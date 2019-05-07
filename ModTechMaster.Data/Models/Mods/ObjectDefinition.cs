using System.Collections.Generic;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    public class ObjectDefinition : JsonObjectSourcedFromFile, IObjectDefinition
    {
        public ObjectDefinition(IObjectDefinitionDescription objectDescription, dynamic jsonObject, string filePath) : base(filePath, (JObject)jsonObject)
        {
            ObjectDescription = objectDescription;
            MetaData = new Dictionary<string, dynamic>();
        }

        public IObjectDefinitionDescription ObjectDescription { get; private set; }
        public Dictionary<string, dynamic> MetaData { get; private set; }

        public virtual string GetId => ObjectDescription?.Id;
    }
}