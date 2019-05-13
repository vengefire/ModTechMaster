using Castle.Core.Logging;
using ModTechMaster.Core.Interfaces.Services;

namespace ModTechMaster.Logic.Services
{
    using System;
    using System.Collections.Generic;
    using Core.Enums;

    public class MessageService : IMessageService
    {
        private readonly ILogger _logger;
        private readonly Queue<Tuple<string, MessageType>> messages;

        public MessageService(ILogger logger)
        {
            _logger = logger;
            this.messages = new Queue<Tuple<string, MessageType>>();
        }

        public void PushMessage(string message, MessageType type)
        {
            _logger.Info($"Added message [{message}]");
            this.messages.Enqueue(new Tuple<string, MessageType>(message, type));
        }

        public Tuple<string, MessageType> PopMessage()
        {
            return this.messages.Dequeue();
        }
    }
}