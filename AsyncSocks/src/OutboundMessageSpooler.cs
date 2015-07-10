using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class OutboundMessageSpooler : ThreadRunner, IOutboundMessageSpooler
    {
        private BlockingCollection<byte[]> queue;

        public OutboundMessageSpooler(IOutboundMessageSpoolerRunnable runnable, BlockingCollection<byte[]> queue) : base(runnable) 
        {
            this.queue = queue;
        }

        public static OutboundMessageSpooler Create(ITcpClient tcpClient)
        {
            BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            OutboundMessageSpoolerRunnable runnable = new OutboundMessageSpoolerRunnable(tcpClient, queue);
            return new OutboundMessageSpooler(runnable, queue);
        }

        public void Enqueue(byte[] messageBytes)
        {
            queue.Add(messageBytes);
        }
    }
}
