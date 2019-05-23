namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    using ModTechMaster.Core.Enums.Mods;

    public interface IManifestEntry : IJsonObjectBase, IReferenceableObjectProvider
    {
        ObjectType EntryType { get; }

        IManifest Manifest { get; }

        HashSet<IObjectDefinition> Objects { get; }

        string Path { get; }
    }
}