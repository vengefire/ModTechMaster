namespace MTM.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Castle.Core.Logging;

    using Framework.Utils.Instrumentation;

    using Newtonsoft.Json;

    public class JsonValidator
    {
        private readonly ILogger logger;

        public JsonValidator(ILogger logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Tuple<string, Exception>> ValidateJsonFiles(string root)
        {
            var invalidJsonFiles = new List<Tuple<string, Exception>>();

            void RecurseDirectories(DirectoryInfo di, int maxDepth, int depth = 0)
            {
                this.logger.Info($"Testing all JSON files in [{di.FullName}]] for validity...");
                di.GetFiles("*.json").AsParallel().ForAll(
                    info =>
                        {
                            try
                            {
                                this.logger.Debug($"Testing JSON file [{info.FullName}]]...");
                                dynamic modConfig = JsonConvert.DeserializeObject(File.ReadAllText(info.FullName));
                            }
                            catch (Exception ex)
                            {
                                this.logger.Warn(
                                    $"JSON file [{info.FullName}]] tested invalid, error = [{ex.ToString()}].");

                                lock (invalidJsonFiles)
                                {
                                    invalidJsonFiles.Add(new Tuple<string, Exception>(info.FullName, ex));
                                }
                            }
                        });

                if (maxDepth == -1 || depth != maxDepth)
                {
                    di.GetDirectories().Where(info => !info.Attributes.HasFlag(FileAttributes.Hidden)).AsParallel().ForAll(subDi => RecurseDirectories(subDi, depth++, maxDepth));
                }
            }

            using (var timer = new ScopedStopwatch(this.logger))
            {
                RecurseDirectories(new DirectoryInfo(root), -1);
            }

            return invalidJsonFiles.AsEnumerable();
        }
    }
}