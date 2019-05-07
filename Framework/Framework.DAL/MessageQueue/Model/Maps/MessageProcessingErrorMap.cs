namespace Framework.Data.MessageQueue.Model.Maps
{
    using DapperExtensions.Mapper;

    public class MessageProcessingErrorMap : ClassMapper<MessageProcessingError>
    {
        public MessageProcessingErrorMap()
        {
            this.Schema("DataExchange");
            this.AutoMap();
        }
    }
}