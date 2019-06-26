namespace ModTechMaster.Data.Models.Mods
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json;

    public abstract class JsonObjectBase : IJsonObjectBase
    {
        public JsonObjectBase(dynamic jsonObject, ObjectType objectType)
        {
            this.JsonObject = jsonObject;
            this.ObjectType = objectType;
        }

        public abstract string Id { get; }

        public dynamic JsonObject { get; }

        public string JsonString
        {
            get => JsonConvert.SerializeObject(this.JsonObject, Formatting.Indented);
        }

        public abstract string Name { get; }

        public ObjectType ObjectType { get; }

        public abstract IValidationResult ValidateObject();
    }
}