namespace ModTechMaster.Logic.Services
{
    using System;
    using Core.Enums;

    public interface IMessageService
    {
        void PushMessage(string message, MessageType type);

        Tuple<string, MessageType> PopMessage();
    }
}