namespace ModTechMaster.Core.Interfaces.Processors
{
    using ModTechMaster.Core.Enums.Mods;
    using ModTechMaster.Core.Interfaces.Models;

    public interface IManifestEntryProcessor
    {
        IManifestEntry ProcessManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject);
    }
}