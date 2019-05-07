namespace Framework.Interfaces.Environment
{
    using Domain;

    public interface IEnvironment
    {
        EnvironmentType EnvironmentType { get; }

        string NedBankNcrRefNo { get; }

        string DevStream { get; }

        string MessageQueueHostServerName { get; }
    }
}