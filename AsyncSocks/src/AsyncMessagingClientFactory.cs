using System.Net;
using System.Net.Sockets;

namespace AsyncSocks
{
    public class AsyncMessagingClientFactory : AsyncClientFactory<byte[]>
    {
        public AsyncMessagingClientFactory(ClientConfig clientConfig) : base(clientConfig) { }
        public AsyncMessagingClientFactory() : base() { }

        public override IAsyncClient<byte[]> Create(IInboundMessageSpooler<byte[]> inboundSpooler, IOutboundMessageSpooler<byte[]> outboundSpooler, IMessagePoller<byte[]> messagePoller, IOutboundMessageFactory<byte[]> messageFactory, ITcpClient tcpClient)
        {
            return new AsyncClient<byte[]>(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient, ClientConfig);
        }

        public override IAsyncClient<byte[]> Create(ITcpClient tcpClient)
        {
            var writer = new NetworkMessageWriter(tcpClient);
            var reader = new NetworkMessageReader(tcpClient, int.Parse(ClientConfig.GetProperty("MaxMessageSize"))); // FIXME: This ClientConfig object here is quite coupled if we plan to use it as a configuration object for every kind of implementation. It should be abstract, or just an interface
            var inboundSpooler = InboundMessageSpooler<byte[]>.Create(reader); 
            var outboundSpooler = OutboundMessageSpooler<byte[]>.Create(writer);
            var messagePoller = MessagePoller<byte[]>.Create(inboundSpooler.Queue);
            var messageFactory = new OutboundMessageFactory<byte[]>();

            return new AsyncMessagingClient(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient, ClientConfig);
        }

        public override IAsyncClient<byte[]> Create(IPEndPoint remoteEndPoint)
        {
            var tcpClient = new BaseTcpClient(new TcpClient(remoteEndPoint.Address.ToString(), remoteEndPoint.Port));
            return Create(tcpClient);
        }

        public override IAsyncClient<byte[]> Create()
        {
            var tcpClient = new BaseTcpClient(new TcpClient());
            return Create(tcpClient);
        }
    }
}
