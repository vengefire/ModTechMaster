namespace Framework.Interfaces.Email
{
    using Domain.Email.Models;

    public delegate bool ProcessEmailEventHandler(Email emailMessage);

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