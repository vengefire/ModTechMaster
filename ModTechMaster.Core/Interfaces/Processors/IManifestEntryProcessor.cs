namespace ModTechMaster.Core.Interfaces.Processors
{
    using Enums.Mods;
    using Models;

    public interface IManifestEntryProcessor
    {
        IManifestEntry ProcessManifestEntry(IManifest manifest, ManifestEntryType entryType, string path, dynamic jsonObject);
    }
}