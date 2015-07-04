using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace AsyncSocks
{
    public class InboundMessageSpooler
    {
        private INetworkMessageReader networkMessageReader;
        private BlockingCollection<byte[]> queue;

        public InboundMessageSpooler(INetworkMessageReader networkMessageReader, BlockingCollection<byte[]> queue)
        {
            // TODO: Complete member initialization
            this.networkMessageReader = networkMessageReader;
            this.queue = queue;
        }

        public void Spool()
        {
            byte[] message = networkMessageReader.Read();
            queue.Add(message);
        }
    }
}
