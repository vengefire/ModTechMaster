using ModTechMaster.Data.Models.Mods;

namespace MTM
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using ModTechMaster.Core.Interfaces.Models;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Factories;
    using ModTechMaster.Logic.Services;

    internal class Program
    {
        private static int Main(string[] args)
        {
            var di = new DirectoryInfo(args[0]);
            if (!di.Exists)
            {
                Console.WriteLine($"The target directory [{di.FullName}] foes not exist.");
                return -1;
            }

            IModService modService = new ModService(new MessageService(), new ManifestEntryProcessorFactory());
            IModCollection modCollection = new ModCollection("MTM Mod Collection");

            Console.WriteLine($"Processing mods from [{di.FullName}]");
            var stopwatch = new Stopwatch();
            di.GetDirectories().ToList().ForEach(
                                                 sub =>
                                                 {
                                                     stopwatch.Start();
                                                     Console.Write($"Processing [{sub.Name}]...");
                                                     modCollection.AddModToCollection(modService.TryLoadFromPath(sub.FullName));
                                                     stopwatch.Stop();
                                                     Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
                                                     stopwatch.Reset();
                                                 });

            var refService = new ReferenceFinderService();
            Console.WriteLine($"Processing Mod Collection object relationships...");
            long elapsedTime = refService.ProcessModCollectionReferences(modCollection);
            Console.WriteLine($"Object relationships processed in [{elapsedTime}] ms.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            return 0;
        }
    }
}