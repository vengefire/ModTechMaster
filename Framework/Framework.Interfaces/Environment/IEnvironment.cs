using Framework.Domain;

namespace Framework.Interfaces.Environment
{
    public interface IEnvironment
    {
        EnvironmentType EnvironmentType { get; }

        string NedBankNcrRefNo { get; }

        string DevStream { get; }

        string MessageQueueHostServerName { get; }
    }
}