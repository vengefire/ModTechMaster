using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Data.Models.Mods
{
    using Core.Interfaces.Models;
    using Newtonsoft.Json;

    public class JsonObjectBase : IJsonObjectBase
    {
        public JsonObjectBase(dynamic jsonObject, ObjectType objectType)
        {
            this.JsonObject = jsonObject;
            ObjectType = objectType;
        }

        public dynamic JsonObject { get; }

        public string JsonString => JsonConvert.SerializeObject(this.JsonObject);
        public ObjectType ObjectType { get; }
    }
}