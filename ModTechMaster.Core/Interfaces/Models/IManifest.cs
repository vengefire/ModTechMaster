namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IManifest : IJsonObjectBase
    {
        IMod Mod { get; }

        HashSet<IManifestEntry> Entries { get; }
    }
}