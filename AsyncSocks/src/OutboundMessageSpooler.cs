using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class OutboundMessageSpooler<T> : ThreadRunner, IOutboundMessageSpooler<T>
    {
        private BlockingCollection<OutboundMessage<T>> queue;

        public OutboundMessageSpooler(IOutboundMessageSpoolerRunnable runnable, BlockingCollection<OutboundMessage<T>> queue) : base(runnable) 
        {
            this.queue = queue;
        }

        public static OutboundMessageSpooler<T> Create(INetworkWriter<T> writer)
        {
            var queue = new BlockingCollection<OutboundMessage<T>>(new ConcurrentQueue<OutboundMessage<T>>());
            var runnable = new OutboundMessageSpoolerRunnable<T>(writer, queue);
            var spooler = new OutboundMessageSpooler<T>(runnable, queue);
            //spooler.ThreadName = "OutboundMessageSpooler "+ tcpClient.Socket.LocalEndPoint;

            return spooler;
        }

        public void Enqueue(OutboundMessage<T> outboundMessage)
        {
            queue.Add(outboundMessage);
        }
    }
}
