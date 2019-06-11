namespace ModTechMaster.Logic.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Castle.Core.Internal;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Data.Models;

    public class FactionService : IFactionService
    {
        private static object lockObject = new object();
        private List<string> factionsList;

        public List<string> GetFactions()
        {
            if (this.factionsList == null)
            {
                lock (FactionService.lockObject)
                {
                    this.factionsList = new List<string>();
                    var assembly = Assembly.GetAssembly(typeof(ObjectReference<>));

                    // var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "ModTechMaster.Data.Resources.factions.csv";
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var line = string.Empty;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (!line.IsNullOrEmpty() && !line.StartsWith("--"))
                                {
                                    this.factionsList.Add(line.Trim());
                                }
                            }
                        }
                    }
                }
            }

            return this.factionsList;
        }
    }
}