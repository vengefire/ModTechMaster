namespace ModTechMaster.Core.Interfaces.Models
{
    using System.Collections.Generic;

    public interface IManifest : IJsonObjectBase, IReferenceableObjectProvider
    {
        HashSet<IManifestEntry> Entries { get; }

        IMod Mod { get; }
    }
}