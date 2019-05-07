using DapperExtensions.Mapper;

namespace Framework.Data.MessageQueue.Model.Maps
{
    public class MessageProcessingErrorMap : ClassMapper<MessageProcessingError>
    {
        public MessageProcessingErrorMap()
        {
            Schema("DataExchange");
            AutoMap();
        }
    }
}