namespace ModTechMaster.Core.Interfaces.Services
{
    using System;
    using Enums;

    public interface IMessageService
    {
        void PushMessage(string message, MessageType type);

        Tuple<string, MessageType> PopMessage();
    }
}