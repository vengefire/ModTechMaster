using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Data.Models.Mods
{
    using Core.Interfaces.Models;
    using Newtonsoft.Json.Linq;

    public class ObjectDefinitionDescription : JsonObjectBase, IObjectDefinitionDescription
    {
        public ObjectDefinitionDescription(string id, string name, string description, string icon, dynamic jsonObject) : base((JObject)jsonObject, ObjectType.ObjectDefinitionDescription)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Icon = icon;
        }

        public ObjectDefinitionDescription(string id, string name, dynamic jsonObject) : base((JObject)jsonObject, ObjectType.ObjectDefinitionDescription)
        {
            this.Id = id;
            this.Name = name;
        }

        public override string Id { get; }

        public override string Name { get; }

        public string Description { get; }

        public string Icon { get; }
    }
}