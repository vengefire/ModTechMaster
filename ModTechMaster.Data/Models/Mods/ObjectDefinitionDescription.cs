namespace ModTechMaster.Data.Models.Mods
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    using Newtonsoft.Json.Linq;

    public class ObjectDefinitionDescription : JsonObjectBase, IObjectDefinitionDescription
    {
        public ObjectDefinitionDescription(string id, string name, string description, string icon, dynamic jsonObject)
            : base((JObject)jsonObject, ObjectType.ObjectDefinitionDescription)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.Icon = icon;
        }

        public ObjectDefinitionDescription(string id, string name, dynamic jsonObject)
            : base((JObject)jsonObject, ObjectType.ObjectDefinitionDescription)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Description { get; }

        public string Icon { get; }

        public override string Id { get; }

        public override string Name { get; }

        public override IValidationResult ValidateObject() => ValidationResult.SuccessValidationResult();

        public static ObjectDefinitionDescription CreateDefault(dynamic description)
        {
            if (description == null || description is JValue)
            {
                return null;
            }

            string id = description.ID != null ? description.ID.ToString() : null;
            string name = description.Name != null ? description.Name.ToString() : null;
            string desc = description.Description != null ? description.Description.ToString() : null;
            string icon = description.Icon != null ? description.Icon.ToString() : null;
            return new ObjectDefinitionDescription(id, name, desc, icon, description);
        }
    }
}