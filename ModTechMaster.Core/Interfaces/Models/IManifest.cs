namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IManifest : IJsonObjectBase, IReferenceableObjectProvider
    {
        IMod Mod { get; }

        HashSet<IManifestEntry> Entries { get; }
    }
}