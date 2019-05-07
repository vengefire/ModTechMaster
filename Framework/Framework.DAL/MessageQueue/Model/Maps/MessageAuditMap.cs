namespace Framework.Data.MessageQueue.Model.Maps
{
    using DapperExtensions.Mapper;

    public class MessageAuditMap : ClassMapper<MessageAudit>
    {
        public MessageAuditMap()
        {
            this.Schema("DataExchange");
            this.AutoMap();
        }
    }
}