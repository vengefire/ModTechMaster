namespace ModTechMaster.Core.Interfaces.Services
{
    using System;
    using System.Collections.ObjectModel;

    using ModTechMaster.Core.Enums;

    public interface IMessageService
    {
        ObservableCollection<string> ErrorMessages { get; }

        ObservableCollection<string> InfoMessages { get; }

        ObservableCollection<string> Messages { get; }

        void ClearMessages();

        Tuple<string, MessageType> PopMessage();

        void PushMessage(string message, MessageType type);
    }
}