using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    /// <summary>
    /// Runs a thread that sends queued messages to the client.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public class OutboundMessageSpooler<T> : ThreadRunner, IOutboundMessageSpooler<T>
    {
        private BlockingCollection<OutboundMessage<T>> queue;

        public OutboundMessageSpooler(IOutboundMessageSpoolerRunnable runnable, BlockingCollection<OutboundMessage<T>> queue) : base(runnable) 
        {
            this.queue = queue;
        }

        /// <summary>
        /// Creates an instance of this class from an instance of INetworkWriter
        /// </summary>
        /// <param name="writer">Writer used to write messages to the network.</param>
        /// <returns>Instance of this class</returns>
        public static OutboundMessageSpooler<T> Create(INetworkWriter<T> writer)
        {
            var queue = new BlockingCollection<OutboundMessage<T>>(new ConcurrentQueue<OutboundMessage<T>>());
            var runnable = new OutboundMessageSpoolerRunnable<T>(writer, queue);
            var spooler = new OutboundMessageSpooler<T>(runnable, queue);
            //spooler.ThreadName = "OutboundMessageSpooler "+ tcpClient.Socket.LocalEndPoint;

            return spooler;
        }

        /// <summary>
        /// Enqueues a message to be sent to the client.
        /// </summary>
        /// <param name="outboundMessage"></param>
        public void Enqueue(OutboundMessage<T> outboundMessage)
        {
            queue.Add(outboundMessage);
        }
    }
}
