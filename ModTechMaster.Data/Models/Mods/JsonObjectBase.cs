using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json;

namespace ModTechMaster.Data.Models.Mods
{
    public abstract class JsonObjectBase : IJsonObjectBase
    {
        public JsonObjectBase(dynamic jsonObject, ObjectType objectType)
        {
            JsonObject = jsonObject;
            ObjectType = objectType;
        }

        public dynamic JsonObject { get; }

        public string JsonString => JsonConvert.SerializeObject(JsonObject, Formatting.Indented);
        public ObjectType ObjectType { get; }
        public abstract string Name { get; }
        public abstract string Id { get; }
    }
}