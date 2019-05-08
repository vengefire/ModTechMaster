namespace ModTechMaster.Core.Interfaces.Models
{
    public interface IJsonObjectBase : IObject
    {
        dynamic JsonObject { get; }

        string JsonString { get; }
    }
}