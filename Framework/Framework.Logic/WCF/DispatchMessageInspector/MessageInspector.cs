namespace Framework.Logic.WCF.DispatchMessageInspector
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.Threading;
    using Castle.Core.Logging;
    using Domain.Services;
    using Interfaces.Data.Services;
    using Interfaces.Injection;

    public class MessageInspector : IDispatchMessageInspector
    {
        private static readonly Dictionary<int, Message> MessageDictionary = new Dictionary<int, Message>();

        private readonly IContainer _container;

        public MessageInspector()
        {
            this._container = Container.Instance;
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var serviceMethodLogService = this._container.GetInstance<IServiceMethodLogService>();
            int threadId;
            int logId;
            try
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
                MessageInspector.MessageDictionary.Add(threadId, request);
                var logEntry = new ServiceMethodLog {Service = request.Headers.To.LocalPath, Method = request.Headers.Action, RequestMessage = request.ToString()};

                logId = serviceMethodLogService.AddLog(ref logEntry);
            }
            finally
            {
                this._container.Release(serviceMethodLogService);
            }

            return new MessageInspectorContext(threadId, logId, DateTime.Now);
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var serviceMethodLogService = this._container.GetInstance<IServiceMethodLogService>();
            try
            {
                var context = (MessageInspectorContext)correlationState;
                var elapsedTime = DateTime.Now - context.InvocationTimeStamp;
                serviceMethodLogService.UpdateLog(context.LogId, reply.ToString(), elapsedTime.Ticks);
                MessageInspector.MessageDictionary.Remove(context.ThreadId);
            }
            finally
            {
                this._container.Release(serviceMethodLogService);
            }
        }

        public static Message GetCurrentThreadContextRequestMessage()
        {
            return MessageInspector.MessageDictionary[Thread.CurrentThread.ManagedThreadId];
        }

        public struct MessageInspectorContext
        {
            public MessageInspectorContext(int threadId, int logId, DateTime invocationTimeStamp)
            {
                this.ThreadId = threadId;
                this.LogId = logId;
                this.InvocationTimeStamp = invocationTimeStamp;
            }

            public int ThreadId { get; set; }

            public int LogId { get; set; }

            public DateTime InvocationTimeStamp;
        }
    }
}