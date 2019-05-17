using ModTechMaster.Core.Enums.Mods;
using ModTechMaster.Core.Interfaces.Models;

namespace ModTechMaster.Data.Models.Mods
{
    public class ResourceDefinition : SourcedFromFile, IResourceDefinition
    {
        public ResourceDefinition(ObjectType objectType, string sourceFilePath, string name, string id) : base(
            sourceFilePath)
        {
            ObjectType = objectType;
            Name = name;
            Id = id;
        }

        public ObjectType ObjectType { get; }
        public string Name { get; }
        public string Id { get; }
    }
}