using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.Core.Logging;
using ModTechMaster.Core.Interfaces.Models;
using ModTechMaster.Core.Interfaces.Services;
using ModTechMaster.Data.Models.Mods;
using MTM.Init;

namespace MTM
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var container = new Bootstrap().RegisterContainer();
            var logger = container.GetInstance<ILogger>();

            var di = new DirectoryInfo(args[0]);
            if (!di.Exists)
            {
                logger.Info($"The target directory [{di.FullName}] foes not exist.");
                return -1;
            }
            var modService = container.GetInstance<IModService>();

            IModCollection modCollection = new ModCollection("MTM Mod Collection", di.FullName);

            logger.Info($"Processing mods from [{di.FullName}]");
            var stopwatch = new Stopwatch();
            di.GetDirectories().ToList().ForEach(
                sub =>
                {
                    stopwatch.Start();
                    logger.Info($"Processing [{sub.Name}]...");
                    modCollection.AddModToCollection(modService.TryLoadFromPath(sub.FullName));
                    stopwatch.Stop();
                    logger.Info($"{stopwatch.ElapsedMilliseconds} ms");
                    stopwatch.Reset();
                });

            var refService = container.GetInstance<IReferenceFinderService>();
            logger.Info("Processing Mod Collection object relationships...");
            var elapsedTime = refService.ProcessModCollectionReferences(modCollection);
            logger.Info($"Object relationships processed in [{elapsedTime}] ms.");

            logger.Info("Press any key to exit.");
            Console.ReadKey();

            return 0;
        }
    }
}