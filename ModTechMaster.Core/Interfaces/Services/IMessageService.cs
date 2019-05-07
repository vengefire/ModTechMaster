using System;
using ModTechMaster.Core.Enums;

namespace ModTechMaster.Core.Interfaces.Services
{
    public interface IMessageService
    {
        void PushMessage(string message, MessageType type);
        Tuple<string, MessageType> PopMessage();
    }
}