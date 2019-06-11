namespace ModTechMaster.Logic.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using ModTechMaster.Core.Interfaces.Services;

    public class FactionService : IFactionService
    {
        public List<string> GetFactions()
        {
            // var assembly = Assembly.GetAssembly(typeof(ObjectReference<>));
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ModTechMaster.Data.Resources.factions.csv";
            var factions = new List<string>();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = string.Empty;
                    while ((line = reader.ReadLine()) != string.Empty)
                    {
                        factions.Add(line);
                    }
                }
            }

            return factions;
        }
    }
}