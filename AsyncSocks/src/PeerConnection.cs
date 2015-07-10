using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class PeerConnection
    {
        private IInboundMessageSpooler inboundSpooler;
        private IOutboundMessageSpooler outboundSpooler;

        public PeerConnection(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler)
        {
            this.inboundSpooler = inboundSpooler;
            this.outboundSpooler = outboundSpooler;
        }

        public void SendMessage(byte[] messageBytes)
        {
            outboundSpooler.Enqueue(messageBytes);
        }
    }
}
