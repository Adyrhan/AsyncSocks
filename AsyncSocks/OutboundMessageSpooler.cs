using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class OutboundMessageSpooler
    {
        private ITcpClient tcpClient;
        private BlockingCollection<byte[]> queue;

        public OutboundMessageSpooler(ITcpClient tcpClient, BlockingCollection<byte[]> queue)
        {
            this.tcpClient = tcpClient;
            this.queue = queue;
        }

        public void Spool()
        {
            byte[] message = queue.Take();
            tcpClient.Write(message, 0, message.Length);
        }
    }
}
