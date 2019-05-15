namespace ModTechMaster.Core.Interfaces.Services
{
    public interface ISettingsService
    {
        void SaveSettings(string name, object settings);
        TType ReadSettings<TType>(string name);
    }
}