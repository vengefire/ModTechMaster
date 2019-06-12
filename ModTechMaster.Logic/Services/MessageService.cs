namespace ModTechMaster.Logic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Castle.Core.Logging;

    using ModTechMaster.Core.Enums;
    using ModTechMaster.Core.Interfaces.Services;
    using ModTechMaster.Logic.Annotations;

    public class MessageService : IMessageService, INotifyPropertyChanged
    {
        private readonly ILogger logger;

        private readonly Queue<Tuple<string, MessageType>> messages;

        public MessageService(ILogger logger)
        {
            this.logger = logger;
            this.messages = new Queue<Tuple<string, MessageType>>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> ErrorMessages => this.Messages;

        public ObservableCollection<string> InfoMessages => this.Messages;

        public ObservableCollection<string> Messages =>
            new ObservableCollection<string>(this.messages.Select(tuple => tuple.Item1));

        public void ClearMessages()
        {
            this.messages.Clear();
            this.OnPropertyChanged(nameof(this.Messages));
        }

        public Tuple<string, MessageType> PopMessage()
        {
            return this.messages.Dequeue();
        }

        public void PushMessage(string message, MessageType type)
        {
            this.logger.Info($"Added message [{message}]");
            this.messages.Enqueue(new Tuple<string, MessageType>(message, type));
            this.OnPropertyChanged(nameof(this.Messages));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}