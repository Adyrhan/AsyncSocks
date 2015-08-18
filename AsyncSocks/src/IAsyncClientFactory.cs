using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IAsyncClientFactory
    {
        IAsyncClient Create(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, IMessagePoller messagePoller, IOutboundMessageFactory messageFactory, ITcpClient tcpClient);
        IAsyncClient Create(ITcpClient tcpClient);
    }
}
