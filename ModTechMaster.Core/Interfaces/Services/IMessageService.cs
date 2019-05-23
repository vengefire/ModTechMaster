namespace ModTechMaster.Core.Interfaces.Services
{
    using System;

    using ModTechMaster.Core.Enums;

    public interface IMessageService
    {
        Tuple<string, MessageType> PopMessage();

        void PushMessage(string message, MessageType type);
    }
}