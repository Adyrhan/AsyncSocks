using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class OutboundMessageSpooler : ThreadRunner
    {
        private OutboundMessageSpoolerRunnable runnable;

        public OutboundMessageSpooler(OutboundMessageSpoolerRunnable runnable) : base(runnable) {}

        public static OutboundMessageSpooler Create(ITcpClient tcpClient)
        {
            BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            OutboundMessageSpoolerRunnable runnable = new OutboundMessageSpoolerRunnable(tcpClient, queue);
            return new OutboundMessageSpooler(runnable);
        }
    }
}
