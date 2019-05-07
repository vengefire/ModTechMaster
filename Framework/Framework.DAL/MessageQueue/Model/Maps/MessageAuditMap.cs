using DapperExtensions.Mapper;

namespace Framework.Data.MessageQueue.Model.Maps
{
    public class MessageAuditMap : ClassMapper<MessageAudit>
    {
        public MessageAuditMap()
        {
            Schema("DataExchange");
            AutoMap();
        }
    }
}