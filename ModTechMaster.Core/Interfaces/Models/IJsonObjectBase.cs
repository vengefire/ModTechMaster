namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IJsonObjectBase
    {
        dynamic JsonObject { get; }

        string JsonString { get; }
    }
}