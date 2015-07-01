using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class ClientMessageSpooler
    {
        private INetworkMessageReader networkMessageReader;
        private System.Collections.Concurrent.BlockingCollection<byte[]> queue;

        public ClientMessageSpooler(INetworkMessageReader networkMessageReader, System.Collections.Concurrent.BlockingCollection<byte[]> queue)
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
