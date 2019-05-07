using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Utils.Directory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MTMScrape
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var path = args[0];
            if (!DirectoryUtils.Exists(path))
            {
                Console.WriteLine($"Path [{path}] does not exist.");
                return -1;
            }

            // Stuff we're scraping...
            var uniqueTypes = new HashSet<string>();
            var invalidJsonFiles = new List<string>();
            void RecurseDirectories(DirectoryInfo di, int maxDepth, int level = 0)
            {
                di.GetFiles("*.json").ToList().ForEach(info =>
                {
                    try
                    {
                        dynamic modConfig = JsonConvert.DeserializeObject(File.ReadAllText(info.FullName));
                        if (info.Name == "mod.json")
                        {
                            if (modConfig.Manifest != null)
                            {
                                ((JArray) modConfig.Manifest).Select(token => (dynamic) token).ToList().ForEach(
                                    token => { uniqueTypes.Add(token.Type.ToString()); });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        invalidJsonFiles.Add(info.FullName);
                    }
                });

                if (maxDepth != -1 || level != maxDepth)
                {
                    di.GetDirectories().ToList().ForEach(subDi => RecurseDirectories(subDi, level++, maxDepth));
                }
            }

            
            RecurseDirectories(new DirectoryInfo(path), -1);

            Console.WriteLine("Manifest Entry Types:");
            Console.WriteLine(string.Join(",\r\n", uniqueTypes.Select(s => s)));
            Console.WriteLine();
            Console.WriteLine("Invalid .json files:");
            Console.WriteLine(string.Join(",\r\n", invalidJsonFiles.Select(s => s)));

            return 0;
        }
    }
}