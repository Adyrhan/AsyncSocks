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
            BlockingCollection<OutboundMessage> queue = new BlockingCollection<OutboundMessage>(new ConcurrentQueue<OutboundMessage>());
            OutboundMessageSpoolerRunnable runnable = new OutboundMessageSpoolerRunnable(tcpClient, queue);
            return new OutboundMessageSpooler(runnable, queue);
        }

        public void Enqueue(OutboundMessage outboundMessage)
        {
            queue.Add(outboundMessage);
        }
    }
}
