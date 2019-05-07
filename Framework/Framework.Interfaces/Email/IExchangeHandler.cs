namespace Framework.Interfaces.Email
{
    public delegate bool ProcessEmailEventHandler(Domain.Email.Models.Email emailMessage);

    public delegate void MailboxMonitoringStartedEventHandler();

    public delegate void MailboxMonitoringStoppedEventHandler();

    public interface IExchangeHandler
    {
        event MailboxMonitoringStartedEventHandler MailboxMonitoringStartedEventHandler;

        event MailboxMonitoringStoppedEventHandler MailboxMonitoringStoppedEventHandler;

        event ProcessEmailEventHandler ProcessEmailEventHandler;

        //// event Func<Email, Task<bool>> ProcessEmailEventHandlerAsync;

        void ConnectToMailbox(string mailbox);

        void StartMonitoring(int pollTimeMilliseconds, int numMessagesPerTick);

        void StopMonitoring();
    }
}