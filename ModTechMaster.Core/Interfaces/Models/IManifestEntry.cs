namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;
    using Enums.Mods;

    public interface IManifestEntry : IJsonObjectBase
    {
        IManifest Manifest { get; }

        ManifestEntryType EntryType { get; }

        string Path { get; }

        HashSet<IObjectDefinition> Objects { get; }
    }
}