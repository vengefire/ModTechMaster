namespace Framework.Logic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration.Install;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading;
    using Castle.Core.Logging;
    using Interfaces.Injection;
    using Interfaces.Logging;
    using Interfaces.Services;

    public static class ServiceProgram
    {
        private static volatile bool run = true;

        private static ILogger logger = NullLogger.Instance;

        private static IContainer Container { get; set; }

        public static int Run(IBootstrap bootstrap, string[] args)
        {
            try
            {
                ServiceProgram.Container = bootstrap.RegisterContainer();
                ServiceProgram.logger = ServiceProgram.Container.GetInstance<ILogger>();
                ServiceProgram.logger.InfoFormat("IOC boot strapped...");

                var exceptionLogger = ServiceProgram.Container.GetInstance<IExceptionLogger>();

                Thread.GetDomain().UnhandledException += (sender, eventArgs) =>
                                                         {
                                                             var ex = (Exception)eventArgs.ExceptionObject;
                                                             ServiceProgram.logger.ErrorFormat(ex, "An unhandled exception was encountered.");
                                                             exceptionLogger.Log(ex);
                                                         };

                if (Environment.UserInteractive)
                {
                    var parameter = string.Concat(args).ToLower(CultureInfo.InvariantCulture);
                    switch (parameter)
                    {
                        case "--install":
                            ManagedInstallerClass.InstallHelper(new[] {Assembly.GetExecutingAssembly().Location});
                            break;
                        case "--uninstall":
                            ManagedInstallerClass.InstallHelper(new[] {"/u", Assembly.GetExecutingAssembly().Location});
                            break;
                        case "--help":
                            ServiceProgram.ShowHelp();
                            break;
                        default:
                            ServiceProgram.RunAsConsole(ServiceProgram.Container.GetInstance<IService>());
                            break;
                    }
                }
                else
                {
                    var servicesToRun = new ServiceBase[] {ServiceProgram.Container.GetInstance<ServiceProxy>()};

                    ServiceProgram.logger.InfoFormat("Executing ServiceBase.Run...");
                    ServiceBase.Run(servicesToRun);
                }
            }
            catch (Exception ex)
            {
                ServiceProgram.logger.ErrorFormat(ex, "Exception encountered in Main, exiting...");
                return -1;
            }

            ServiceProgram.logger.InfoFormat("Cleanly exiting application.");

            return 0;
        }

        private static void RunAsConsole(IService service)
        {
            Thread.GetDomain().UnhandledException += (sender, args) =>
                                                     {
                                                         var ex = (Exception)args.ExceptionObject;
                                                         ServiceProgram.logger.ErrorFormat(ex, "An unhandled exception was encountered.");
                                                     };

            Console.CancelKeyPress += (sender, args) =>
                                      {
                                          args.Cancel = true;
                                          ServiceProgram.run = false;
                                      };

            ServiceProgram.logger.Info("Interactive mode initiated");
            ServiceProgram.logger.InfoFormat("Start with execute \"{0} --help\" for options.", AppDomain.CurrentDomain.FriendlyName);

            service.OnStart();

            ServiceProgram.logger.Info("Press CTRL+C to exit...");

            while (ServiceProgram.run) Thread.Sleep(1);

            ServiceProgram.logger.Info("Shutting down...");
            service.OnStop();
            ServiceProgram.logger.Info("Done");
        }

        private static void ShowHelp()
        {
            var help = new Dictionary<string, string> {{"install", "Install service"}, {"uninstall", "Uninstall service"}, {"console", "Run in console mode"}, {"help", "This help page"}};

            var maxKeyLength = help.Keys.Max(x => x.Length);
            foreach (var item in help) Console.WriteLine("--{0}\t{1}", item.Key.PadRight(maxKeyLength, ' '), item.Value);
        }
    }
}