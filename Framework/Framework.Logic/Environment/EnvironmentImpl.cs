using System.Configuration;
using Castle.Core.Internal;
using Framework.Domain;
using Framework.Interfaces.Environment;

namespace Framework.Logic.Environment
{
    public class EnvironmentImpl : IEnvironment
    {
        public EnvironmentImpl()
        {
            var env = ConfigurationManager.AppSettings["Environment"];
            if (!env.IsNullOrEmpty() && env == "Production")
            {
                EnvironmentType = EnvironmentType.Production;
            }
            else
            {
                EnvironmentType = EnvironmentType.Dev;
            }

            var devStream = ConfigurationManager.AppSettings["DevStream"];
            DevStream = devStream ?? string.Empty;
            MessageQueueHostServerName = ConfigurationManager.AppSettings["MessageQueueHostServerName"];
        }

        public EnvironmentType EnvironmentType { get; }

        public string MessageQueueHostServerName { get; }

        public string NedBankNcrRefNo
        {
            get { return "NCRCP16"; }
        }

        public string DevStream { get; }
    }
}