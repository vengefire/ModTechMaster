namespace ModTechMaster.Logic.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Castle.Core.Logging;

    using ModTechMaster.Data.Models.Mods;

    using Newtonsoft.Json;

    internal static class LoadOrder
    {
        public static List<string> CreateLoadOrder(Dictionary<string, ModDef> modDefs, out List<string> unloaded, List<string> cachedOrder, ILogger logger)
        {
            var modDefsCopy = new Dictionary<string, ModDef>(modDefs);
            var loadOrder = new List<string>();

            // remove all mods that have a conflict
            var tryToLoad = modDefs.Keys.ToList();
            var hasConflicts = new List<string>();
            foreach (var modDef in modDefs.Values)
            {
                if (!modDef.HasConflicts(tryToLoad))
                {
                    continue;
                }

                logger.Warn($"Mod [{modDef.Name}] has conflicts and will not load.");
                modDefsCopy.Remove(modDef.Name);
                hasConflicts.Add(modDef.Name);
            }

            FillInOptionalDependencies(modDefsCopy);

            // load the order specified in the file
            foreach (var modName in cachedOrder)
            {
                if (!modDefsCopy.ContainsKey(modName) || !modDefsCopy[modName].AreDependenciesResolved(loadOrder))
                {
                    logger.Warn($"Mod [{modName}] is missing dependencies. It will not be loaded.");
                    continue;
                }

                modDefsCopy.Remove(modName);
                loadOrder.Add(modName);
            }

            // everything that is left in the copy hasn't been loaded before
            unloaded = new List<string>();
            unloaded.AddRange(modDefsCopy.Keys.OrderByDescending(x => x).ToList());

            // there is nothing left to load
            if (modDefsCopy.Count == 0)
            {
                unloaded.AddRange(hasConflicts);
                return loadOrder;
            }

            // this is the remainder that haven't been loaded before
            int removedThisPass;
            do
            {
                removedThisPass = 0;

                for (var i = unloaded.Count - 1; i >= 0; i--)
                {
                    var modDef = modDefs[unloaded[i]];

                    if (!modDef.AreDependenciesResolved(loadOrder))
                    {
                        continue;
                    }

                    unloaded.RemoveAt(i);
                    loadOrder.Add(modDef.Name);
                    removedThisPass++;
                }
            }
            while (removedThisPass > 0 && unloaded.Count > 0);

            unloaded.AddRange(hasConflicts);
            return loadOrder;
        }

        public static List<string> FromFile(string path)
        {
            List<string> order;

            if (File.Exists(path))
            {
                try
                {
                    order = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(path));
                    return order;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // create a new one if it doesn't exist or couldn't be added
            order = new List<string>();
            return order;
        }

        public static void ToFile(List<string> order, string path)
        {
            if (order == null)
            {
                return;
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(order, Formatting.Indented));
        }

        private static void FillInOptionalDependencies(Dictionary<string, ModDef> modDefs)
        {
            // add optional dependencies if they are present
            foreach (var modDef in modDefs.Values)
            {
                if (modDef.OptionallyDependsOn.Count == 0)
                {
                    continue;
                }

                foreach (var optDep in modDef.OptionallyDependsOn)
                {
                    if (modDefs.ContainsKey(optDep))
                    {
                        modDef.DependsOn.Add(optDep);
                    }
                }
            }
        }
    }
}
