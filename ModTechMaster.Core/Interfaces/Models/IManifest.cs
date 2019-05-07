using System.Collections.Generic;

namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IManifest : IJsonObjectBase
    {
        IMod Mod { get; }
        HashSet<IManifestEntry> Entries { get; }
    }
}