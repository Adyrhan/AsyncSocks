using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class InboundMessageSpooler : ThreadRunner, IInboundMessageSpooler
    {
        public static InboundMessageSpooler Create(ITcpClient tcpClient)
        {
            NetworkMessageReader reader = new NetworkMessageReader(tcpClient);
            BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            InboundMessageSpoolerRunnable runnable = new InboundMessageSpoolerRunnable(reader, queue);
            return new InboundMessageSpooler(runnable);
        }

        public InboundMessageSpooler(IInboundMessageSpoolerRunnable runnable) : base(runnable) { }

    }
}
