namespace MTM
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Castle.Core.Internal;
    using Castle.Core.Logging;

    using CommandLine;

    using Framework.Interfaces.Injection;
    using Framework.Logic.Services;

    using ModTechMaster.Core.Interfaces.Services;

    using MTM.Commands;
    using MTM.Init;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new Bootstrap().RegisterContainer();

            Parser.Default.ParseArguments<Options>(args).WithParsed(options => { Run(options, container); });
        }

        private static int Run(Options options, IContainer container)
        {
            var logger = container.GetInstance<ILogger>();

            Thread.GetDomain().UnhandledException += (sender, eventArgs) =>
                {
                    var ex = (Exception)eventArgs.ExceptionObject;
                    logger.ErrorFormat(ex, "An unhandled exception was encountered.");
                };

            logger.Info($"BattleTech Directory : [{options.BattleTechDirectory}]");
            logger.Info($"Mods Directory : [{options.ModsDirectory}]");
            logger.Info($"Pre-validate JSON : [{options.ValidateJson}]");
            logger.Info($"Validate Mods References : [{options.ValidateRefs}]");

            var modsDirectoryInfo = new DirectoryInfo(options.ModsDirectory);
            if (!modsDirectoryInfo.Exists)
            {
                logger.Error($"The target Mods directory [{modsDirectoryInfo.FullName}] does not exist.");
                return -1;
            }

            if (options.ValidateJson)
            {
                var jsonValidator = new JsonValidator(logger);
                var invalidJsonFiles = jsonValidator.ValidateJsonFiles(options.ModsDirectory).ToList();
                if (invalidJsonFiles.Any())
                {
                    logger.Error("Invalid JSON files were found:");
                    Console.WriteLine(
                        string.Join("\r\n", invalidJsonFiles.Select(tuple => $"[{tuple.Item1}] - {tuple.Item2}")));
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return -1;
                }
                logger.Info($"No invalid JSON files found... Will miracles never cease?");
            }

            if (!options.BattleTechDirectory.IsNullOrEmpty())
            {
                var btDirectoryInfo = new DirectoryInfo(options.BattleTechDirectory);
                if (!btDirectoryInfo.Exists)
                {
                    logger.Error($"The target Battle Tech directory [{btDirectoryInfo.FullName}] does not exist.");
                    return -1;
                }
            }

            var modService = container.GetInstance<IModService>();

            logger.Info("Processing mods....");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var modCollection = modService.LoadCollectionFromPath(
                options.BattleTechDirectory,
                modsDirectoryInfo.FullName,
                "MTM Console Collection");
            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            logger.Info($"Mods processed in [{elapsedTime}] ms.");

            var refService = container.GetInstance<IReferenceFinderService>();
            refService.ReferenceableObjectProvider = modCollection;
            logger.Info("Processing Mod Collection object relationships...");
            elapsedTime = refService.ProcessModCollectionReferences(modCollection);
            logger.Info($"Object relationships processed in [{elapsedTime}] ms.");

            if (options.ValidateRefs)
            {
                logger.Info("Running validation procedure...");
                var validateResult = modCollection.ValidateMods();
                logger.Info($"Validation Result : {validateResult.Result}");
                logger.Warn(
                    string.Join(
                        "\r\n",
                        validateResult.ValidationResultReasons.Select(
                            reason => $"[{reason.FailureReason}")));
                logger.Info($"Total Validation Failures = [{validateResult.ValidationResultReasons.Count}]");
            }

            if (options.ValidateLances)
            {
                logger.Info("Running lance validation procedure...");
                var validateResult = modCollection.ValidateLances();
                logger.Info($"Validation Result : {validateResult.Result}");
                logger.Warn(
                    string.Join(
                        "\r\n",
                        validateResult.ValidationResultReasons.Select(
                            reason => $"[{reason.FailureReason}")));
                logger.Info($"Total Lance Validation Failures = [{validateResult.ValidationResultReasons.Count}]");
            }

            logger.Info("Press any key to exit.");
            Console.ReadKey();

            return 0;
        }
    }
}