namespace Framework.Logic.Environment
{
    using System.Configuration;
    using Castle.Core.Internal;
    using Domain;
    using Interfaces.Environment;

    public class EnvironmentImpl : IEnvironment
    {
        public EnvironmentImpl()
        {
            var env = ConfigurationManager.AppSettings["Environment"];
            if (!env.IsNullOrEmpty() &&
                env == "Production")
            {
                this.EnvironmentType = EnvironmentType.Production;
            }
            else
            {
                this.EnvironmentType = EnvironmentType.Dev;
            }

            var devStream = ConfigurationManager.AppSettings["DevStream"];
            this.DevStream = devStream ?? string.Empty;
            this.MessageQueueHostServerName = ConfigurationManager.AppSettings["MessageQueueHostServerName"];
        }

        public EnvironmentType EnvironmentType { get; }

        public string MessageQueueHostServerName { get; }

        public string NedBankNcrRefNo => "NCRCP16";

        public string DevStream { get; }
    }
}