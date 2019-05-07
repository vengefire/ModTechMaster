using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Castle.Core.Logging;
using Framework.Domain.Services;
using Framework.Interfaces.Data.Services;
using Framework.Interfaces.Injection;

namespace Framework.Logic.WCF.DispatchMessageInspector
{
    public class MessageInspector : IDispatchMessageInspector
    {
        private static readonly Dictionary<int, Message> MessageDictionary = new Dictionary<int, Message>();

        private readonly IContainer _container;

        public MessageInspector()
        {
            _container = Container.Instance;
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var serviceMethodLogService = _container.GetInstance<IServiceMethodLogService>();
            int threadId;
            int logId;
            try
            {
                threadId = Thread.CurrentThread.ManagedThreadId;
                MessageDictionary.Add(threadId, request);
                var logEntry = new ServiceMethodLog
                {
                    Service = request.Headers.To.LocalPath,
                    Method = request.Headers.Action,
                    RequestMessage = request.ToString()
                };

                logId = serviceMethodLogService.AddLog(ref logEntry);
            }
            finally
            {
                _container.Release(serviceMethodLogService);
            }

            return new MessageInspectorContext(threadId, logId, DateTime.Now);
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var serviceMethodLogService = _container.GetInstance<IServiceMethodLogService>();
            try
            {
                var context = (MessageInspectorContext) correlationState;
                var elapsedTime = DateTime.Now - context.InvocationTimeStamp;
                serviceMethodLogService.UpdateLog(context.LogId, reply.ToString(), elapsedTime.Ticks);
                MessageDictionary.Remove(context.ThreadId);
            }
            finally
            {
                _container.Release(serviceMethodLogService);
            }
        }

        public static Message GetCurrentThreadContextRequestMessage()
        {
            return MessageDictionary[Thread.CurrentThread.ManagedThreadId];
        }

        public struct MessageInspectorContext
        {
            public MessageInspectorContext(int threadId, int logId, DateTime invocationTimeStamp)
            {
                ThreadId = threadId;
                LogId = logId;
                InvocationTimeStamp = invocationTimeStamp;
            }

            public int ThreadId { get; set; }
            public int LogId { get; set; }
            public DateTime InvocationTimeStamp;
        }
    }
}