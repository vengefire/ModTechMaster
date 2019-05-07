using ModTechMaster.Core.Interfaces.Models;
using Newtonsoft.Json.Linq;

namespace ModTechMaster.Data.Models.Mods
{
    public class ObjectDefinitionDescription : JsonObjectBase, IObjectDefinitionDescription
    {
        public ObjectDefinitionDescription(string id, string name, string description, string icon, dynamic jsonObject) : base((JObject)jsonObject)
        {
            Id = id;
            Name = name;
            Description = description;
            Icon = icon;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Icon { get; private set; }
    }
}