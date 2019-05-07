using System;
using System.Collections.Generic;
using ModTechMaster.Core.Enums;

namespace ModTechMaster.Logic.Services
{
    public class MessageService : IMessageService
    {
        private readonly Queue<Tuple<string, MessageType>> _messages;

        public MessageService()
        {
            _messages = new Queue<Tuple<string, MessageType>>();
        }

        public void PushMessage(string message, MessageType type)
        {
            _messages.Enqueue(new Tuple<string, MessageType>(message, type));
        }

        public Tuple<string, MessageType> PopMessage()
        {
            return _messages.Dequeue();
        }
    }
}