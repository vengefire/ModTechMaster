namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;
    using Enums.Mods;

    public interface IManifestEntry : IJsonObjectBase, IReferenceableObjectProvider
    {
        IManifest Manifest { get; }

        ObjectType EntryType { get; }

        string Path { get; }

        HashSet<IObjectDefinition> Objects { get; }
    }
}