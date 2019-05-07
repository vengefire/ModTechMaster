namespace Framework.Interfaces.Queue
{
    public interface IMessageProcessingLogic<TRequestType>
        where TRequestType : class
    {
        void DoWork(TRequestType message, string messageId, string correlationId);
    }
}