using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class PeerConnectionFactory : IPeerConnectionFactory
    {
        public IPeerConnection Create(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, IMessagePoller messagePoller, ITcpClient tcpClient)
        {
            return new PeerConnection(inboundSpooler, outboundSpooler, messagePoller, tcpClient);
        }

    }
}
