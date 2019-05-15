namespace ModTechMaster.Logic.Services
{
    using System;
    using System.IO;
    using Core.Interfaces.Services;
    using Newtonsoft.Json;

    public class SettingsService : ISettingsService
    {
        public void SaveSettings(string name, object settings)
        {
            File.WriteAllText($"./{name}.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public TType ReadSettings<TType>(string name)
        {
            if (!File.Exists(this.SettingsFileName("./", name)))
            {
                return Activator.CreateInstance<TType>();
            }

            return JsonConvert.DeserializeObject<TType>(File.ReadAllText($"./{name}.json"));
        }

        private string SettingsFileName(string path, string name) => Path.Combine(path, $"{name}.json");
    }
}