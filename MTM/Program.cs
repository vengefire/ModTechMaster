namespace MTM
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Services;

    using MTM.Init;

    internal class Program
    {
        private static int Main(string[] args)
        {
            var container = new Bootstrap().RegisterContainer();
            var logger = container.GetInstance<ILogger>();

            var modsDirectoryInfo = new DirectoryInfo(args[0]);
            if (!modsDirectoryInfo.Exists)
            {
                logger.Info($"The target Mods directory [{modsDirectoryInfo.FullName}] does not exist.");
                return -1;
            }

            var btDirectoryInfo = new DirectoryInfo(args[1]);
            if (!btDirectoryInfo.Exists)
            {
                logger.Info($"The target Battle Tech directory [{btDirectoryInfo.FullName}] does not exist.");
                return -1;
            }

            var modService = container.GetInstance<IModService>();

            logger.Info($"Processing mods from [{modsDirectoryInfo.FullName}]");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var modCollection = modService.LoadCollectionFromPath(btDirectoryInfo.FullName, modsDirectoryInfo.FullName, "MTM Mod Collection");
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            logger.Info($"Mods processed in [{elapsedTime}] ms.");

            var refService = container.GetInstance<IReferenceFinderService>();
            refService.ReferenceableObjectProvider = modCollection;
            logger.Info("Processing Mod Collection object relationships...");
            elapsedTime = refService.ProcessModCollectionReferences(modCollection);
            logger.Info($"Object relationships processed in [{elapsedTime}] ms.");

            logger.Info("Running validation procedure...");
            var validateResult = modCollection.ValidateMods();

            logger.Info("Press any key to exit.");
            Console.ReadKey();

            return 0;
        }
    }
}