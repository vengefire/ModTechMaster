namespace ModTechMaster.Logic.Services
{
    using System;
    using System.Collections.Generic;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Services;

    public class MessageService : IMessageService
    {
        private readonly ILogger logger;

        private readonly Queue<Tuple<string, MessageType>> messages;

        public MessageService(ILogger logger)
        {
            this.logger = logger;
            this.messages = new Queue<Tuple<string, MessageType>>();
        }

        public Tuple<string, MessageType> PopMessage()
        {
            return this.messages.Dequeue();
        }

        public void PushMessage(string message, MessageType type)
        {
            this.logger.Info($"Added message [{message}]");
            this.messages.Enqueue(new Tuple<string, MessageType>(message, type));
        }
    }
}