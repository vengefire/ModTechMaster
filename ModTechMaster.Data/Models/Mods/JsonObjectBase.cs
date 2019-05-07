using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json;

namespace ModTechMaster.Data.Models.Mods
{
    public class JsonObjectBase : IJsonObjectBase
    {
        public JsonObjectBase(dynamic jsonObject)
        {
            JsonObject = jsonObject;
        }

        public dynamic JsonObject { get; }
        public string JsonString => JsonConvert.SerializeObject(this.JsonObject);
    }
}