using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class AsyncClientFactory : IAsyncClientFactory
    {
        public IAsyncClient Create(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, IMessagePoller messagePoller, IOutboundMessageFactory messageFactory, ITcpClient tcpClient)
        {
            return new AsyncClient(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient);
        }

        public IAsyncClient Create(ITcpClient tcpClient, int maxMessageSize)
        {
            var inboundSpooler = InboundMessageSpooler.Create(tcpClient, maxMessageSize);
            var outboundSpooler = OutboundMessageSpooler.Create(tcpClient);
            var messagePoller = MessagePoller.Create(inboundSpooler.Queue);
            var messageFactory = new OutboundMessageFactory();

            return new AsyncClient(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient);
        }

        public IAsyncClient Create(ITcpClient tcpClient)
        {
            return Create(tcpClient, ClientConfig.GetDefault().MaxMessageSize);
        }

    }
}
