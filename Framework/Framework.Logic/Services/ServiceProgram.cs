using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Castle.Core.Logging;
using Framework.Interfaces.Injection;
using Framework.Interfaces.Logging;
using Framework.Interfaces.Services;

namespace Framework.Logic.Services
{
    public static class ServiceProgram
    {
        private static volatile bool run = true;

        private static ILogger logger = NullLogger.Instance;

        private static IContainer Container { get; set; }

        public static int Run(IBootstrap bootstrap, string[] args)
        {
            try
            {
                Container = bootstrap.RegisterContainer();
                logger = Container.GetInstance<ILogger>();
                logger.InfoFormat("IOC boot strapped...");

                var exceptionLogger = Container.GetInstance<IExceptionLogger>();

                Thread.GetDomain().UnhandledException += (sender, eventArgs) =>
                {
                    var ex = (Exception) eventArgs.ExceptionObject;
                    logger.ErrorFormat(ex, "An unhandled exception was encountered.");
                    exceptionLogger.Log(ex);
                };

                if (System.Environment.UserInteractive)
                {
                    var parameter = string.Concat(args).ToLower(CultureInfo.InvariantCulture);
                    switch (parameter)
                    {
                        case "--install":
                            ManagedInstallerClass.InstallHelper(
                                new[]
                                {
                                    Assembly.GetExecutingAssembly().Location
                                });
                            break;
                        case "--uninstall":
                            ManagedInstallerClass.InstallHelper(
                                new[]
                                {
                                    "/u",
                                    Assembly.GetExecutingAssembly().Location
                                });
                            break;
                        case "--help":
                            ShowHelp();
                            break;
                        default:
                            RunAsConsole(Container.GetInstance<IService>());
                            break;
                    }
                }
                else
                {
                    var servicesToRun = new ServiceBase[]
                    {
                        Container.GetInstance<ServiceProxy>()
                    };

                    logger.InfoFormat("Executing ServiceBase.Run...");
                    ServiceBase.Run(servicesToRun);
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat(ex, "Exception encountered in Main, exiting...");
                return -1;
            }

            logger.InfoFormat("Cleanly exiting application.");

            return 0;
        }

        private static void RunAsConsole(IService service)
        {
            Thread.GetDomain().UnhandledException += (sender, args) =>
            {
                var ex = (Exception) args.ExceptionObject;
                logger.ErrorFormat(ex, "An unhandled exception was encountered.");
            };

            Console.CancelKeyPress += (sender, args) =>
            {
                args.Cancel = true;
                run = false;
            };

            logger.Info("Interactive mode initiated");
            logger.InfoFormat("Start with execute \"{0} --help\" for options.", AppDomain.CurrentDomain.FriendlyName);

            service.OnStart();

            logger.Info("Press CTRL+C to exit...");

            while (run)
            {
                Thread.Sleep(1);
            }

            logger.Info("Shutting down...");
            service.OnStop();
            logger.Info("Done");
        }

        private static void ShowHelp()
        {
            var help = new Dictionary<string, string>
            {
                {
                    "install", "Install service"
                },
                {
                    "uninstall", "Uninstall service"
                },
                {
                    "console", "Run in console mode"
                },
                {
                    "help", "This help page"
                }
            };

            var maxKeyLength = help.Keys.Max(x => x.Length);
            foreach (var item in help)
            {
                Console.WriteLine("--{0}\t{1}", item.Key.PadRight(maxKeyLength, ' '), item.Value);
            }
        }
    }
}