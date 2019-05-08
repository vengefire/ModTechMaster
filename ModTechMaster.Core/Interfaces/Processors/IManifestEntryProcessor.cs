namespace ModTechMaster.Core.Interfaces.Processors
{
    using Enums.Mods;
    using Models;

    public interface IManifestEntryProcessor
    {
        IManifestEntry ProcessManifestEntry(IManifest manifest, ObjectType entryType, string path, dynamic jsonObject);
    }
}