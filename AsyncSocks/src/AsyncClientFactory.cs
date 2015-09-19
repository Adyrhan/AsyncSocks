using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class AsyncClientFactory : IAsyncClientFactory
    {
        public AsyncClientFactory(ClientConfig clientConfig)
        {
            ClientConfig = clientConfig;
        }

        public AsyncClientFactory()
        {
            ClientConfig = ClientConfig.GetDefault();
        }

        public ClientConfig ClientConfig { get; }

        public IAsyncClient Create(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, IMessagePoller messagePoller, IOutboundMessageFactory messageFactory, ITcpClient tcpClient)
        {
            return new AsyncClient(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient, ClientConfig);
        }

        public IAsyncClient Create(ITcpClient tcpClient)
        {
            var inboundSpooler = InboundMessageSpooler.Create(tcpClient, ClientConfig.MaxMessageSize);
            var outboundSpooler = OutboundMessageSpooler.Create(tcpClient);
            var messagePoller = MessagePoller.Create(inboundSpooler.Queue);
            var messageFactory = new OutboundMessageFactory();

            return new AsyncClient(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient, ClientConfig);
        }

    }
}
