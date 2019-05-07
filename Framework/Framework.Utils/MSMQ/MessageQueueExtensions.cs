using System;
using System.ComponentModel;
using System.Messaging;
using System.Transactions;

namespace Framework.Utils.MSMQ
{
    public static class MessageQueueExtensions
    {
        public static void MoveToSubQueue(
            this MessageQueue queue,
            string subQueueName,
            Message message)
        {
            string destQueueName;
            if (!string.IsNullOrEmpty(subQueueName))
            {
                destQueueName = @"DIRECT=OS:.\" + queue.QueueName + ";" + subQueueName;
            }
            else
            {
                destQueueName = @"DIRECT=OS:.\" + queue.QueueName.Remove(queue.QueueName.LastIndexOf(';'));
            }

            var queueHandle = IntPtr.Zero;
            var error = NativeMethods.MQOpenQueue(
                destQueueName,
                NativeMethods.MQ_MOVE_ACCESS,
                NativeMethods.MQ_DENY_NONE,
                ref queueHandle);
            if (error != 0)
            {
                throw new InvalidProgramException(
                    "Failed to open queue: " + destQueueName,
                    new Win32Exception(error));
            }

            try
            {
                var current = Transaction.Current;
                IDtcTransaction transaction = null;
                if (current != null && queue.Transactional)
                {
                    transaction = TransactionInterop.GetDtcTransaction(current);
                }

                error = NativeMethods.MQMoveMessage(
                    queue.ReadHandle,
                    queueHandle,
                    message.LookupId,
                    transaction);
                if (error != 0)
                {
                    throw new InvalidProgramException(
                        "Failed to move message to queue: " + destQueueName,
                        new Win32Exception(error));
                }
            }
            finally
            {
                error = NativeMethods.MQCloseQueue(queueHandle);
                if (error != 0)
                {
                    throw new InvalidProgramException(
                        "Failed to close queue: " + destQueueName,
                        new Win32Exception(error));
                }
            }
        }

        public static void MoveToSubQueue(
            string srcQueueName,
            string destQueueName,
            Message message,
            bool isTransactional)
        {
            var targetQueue = IntPtr.Zero;

            var error = NativeMethods.MQOpenQueue(
                destQueueName,
                (int) NativeMethods.MqAccess.Move,
                NativeMethods.MQ_DENY_NONE,
                ref targetQueue);

            if (error != 0)
            {
                throw new InvalidProgramException(
                    "Failed to open queue: " + destQueueName,
                    new Win32Exception(error));
            }

            var srcQueue = IntPtr.Zero;

            error = NativeMethods.MQOpenQueue(
                srcQueueName,
                (int) NativeMethods.MqAccess.Receive,
                NativeMethods.MQ_DENY_NONE,
                ref srcQueue);

            if (error != 0)
            {
                throw new InvalidProgramException(
                    "Failed to open queue: " + srcQueueName,
                    new Win32Exception(error));
            }

            try
            {
                var current = Transaction.Current;
                IDtcTransaction transaction = null;
                if (current != null && isTransactional)
                {
                    transaction = TransactionInterop.GetDtcTransaction(current);
                }

                error = NativeMethods.MQMoveMessage(
                    srcQueue,
                    targetQueue,
                    message.LookupId,
                    transaction);
                if (error != 0)
                {
                    throw new InvalidProgramException(
                        "Failed to move message to queue: " + destQueueName,
                        new Win32Exception(error));
                }
            }
            finally
            {
                NativeMethods.MQCloseQueue(targetQueue);
                NativeMethods.MQCloseQueue(srcQueue);
            }
        }
    }
}