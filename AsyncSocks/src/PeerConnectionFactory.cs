using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class PeerConnectionFactory : IPeerConnectionFactory
    {
        public IPeerConnection Create(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, ITcpClient tcpClient)
        {
            return new PeerConnection(InboundMessageSpooler.Create(tcpClient), OutboundMessageSpooler.Create(tcpClient), tcpClient);
        }
    }
}
