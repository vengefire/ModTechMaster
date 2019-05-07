using System;

namespace Framework.Data.MessageQueue.Model
{
    public class MessageAudit
    {
        public long Id { get; set; }

        public string MessageId { get; set; }

        public string CorrelationId { get; set; }

        public string QueueName { get; set; }

        public string QueuePath { get; set; }

        public string MessageContent { get; set; }

        public int MessageStatusId { get; set; }

        public long ProcessingTime { get; set; }

        public DateTime TmStamp { get; set; }
    }
}