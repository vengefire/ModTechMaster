namespace ModTechMaster.Logic.Services
{
    using System;
    using System.Collections.Generic;
    using Core.Enums;

    public class MessageService : IMessageService
    {
        private readonly Queue<Tuple<string, MessageType>> messages;

        public MessageService()
        {
            this.messages = new Queue<Tuple<string, MessageType>>();
        }

        public void PushMessage(string message, MessageType type)
        {
            this.messages.Enqueue(new Tuple<string, MessageType>(message, type));
        }

        public Tuple<string, MessageType> PopMessage()
        {
            return this.messages.Dequeue();
        }
    }
}