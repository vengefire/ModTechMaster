namespace Framework.Domain.Queue
{
    public enum MessageStatus
    {
        AwaitingProcessing = 0,

        Processed = 1,

        Errored = 2
    }
}