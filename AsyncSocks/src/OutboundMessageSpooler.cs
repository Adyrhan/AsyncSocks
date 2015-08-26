using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class OutboundMessageSpooler : ThreadRunner, IOutboundMessageSpooler
    {
        private BlockingCollection<OutboundMessage> queue;

        public OutboundMessageSpooler(IOutboundMessageSpoolerRunnable runnable, BlockingCollection<OutboundMessage> queue) : base(runnable) 
        {
            this.queue = queue;
        }

        public static OutboundMessageSpooler Create(ITcpClient tcpClient)
        {
            var queue = new BlockingCollection<OutboundMessage>(new ConcurrentQueue<OutboundMessage>());
            var runnable = new OutboundMessageSpoolerRunnable(tcpClient, queue);
            var spooler = new OutboundMessageSpooler(runnable, queue);
            spooler.ThreadName = "OutboundMessageSpooler "+ tcpClient.Socket.LocalEndPoint;

            return spooler;
        }

        public void Enqueue(OutboundMessage outboundMessage)
        {
            queue.Add(outboundMessage);
        }
    }
}
