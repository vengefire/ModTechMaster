using System;

namespace Framework.Data.MessageQueue.Model
{
    public class MessageProcessingError
    {
        public long Id { get; set; }

        public long MessageAuditId { get; set; }

        public string Error { get; set; }

        public string StackTrace { get; set; }

        public DateTime TmStamp { get; set; }
    }
}