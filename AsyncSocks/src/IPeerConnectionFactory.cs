using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IPeerConnectionFactory
    {
        IAsyncClient Create(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, IMessagePoller messagePoller, ITcpClient tcpClient);
        IAsyncClient Create(ITcpClient tcpClient);
    }
}
