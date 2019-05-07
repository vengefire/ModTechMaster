using System.Collections.Generic;
using ModTechMaster.Core.Enums.Mods;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IManifestEntry : IJsonObjectBase
    {
        IManifest Manifest { get; }
        ManifestEntryType EntryType { get; }
        string Path { get; }
        HashSet<IObjectDefinition> Objects { get; }
    }
}