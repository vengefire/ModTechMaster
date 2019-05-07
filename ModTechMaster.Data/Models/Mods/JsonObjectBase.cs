namespace ModTechMaster.Data.Models.Mods
{
    using Core.Interfaces.Models;
    using Newtonsoft.Json;

    public class JsonObjectBase : IJsonObjectBase
    {
        public JsonObjectBase(dynamic jsonObject)
        {
            this.JsonObject = jsonObject;
        }

        public dynamic JsonObject { get; }

        public string JsonString => JsonConvert.SerializeObject(this.JsonObject);
    }
}