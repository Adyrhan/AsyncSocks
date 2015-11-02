using System.Net;
using System.Net.Sockets;

namespace AsyncSocks.AsyncMessaging
{
    /// <summary>
    /// Implementation of AsyncClientFactory that creates instances of AsyncMessagingClient <see cref="AsyncMessagingClient"/>
    /// </summary>
    public class AsyncMessagingClientFactory : AsyncClientFactory<byte[]>
    {
        /// <inheritdoc />
        public AsyncMessagingClientFactory(ClientConfig clientConfig) : base(clientConfig) { }

        /// <summary>
        /// This version of the constructor will use the default ClientConfig instance from AsyncMessagingClientConfig.GetDefault()
        /// </summary>
        public AsyncMessagingClientFactory() : base(AsyncMessagingClientConfig.GetDefault()) { }

        /// <inheritdoc />
        public override IAsyncClient<byte[]> Create(IInboundMessageSpooler<byte[]> inboundSpooler, IOutboundMessageSpooler<byte[]> outboundSpooler, IMessagePoller<byte[]> messagePoller, IOutboundMessageFactory<byte[]> messageFactory, ITcpClient tcpClient)
        {
            return new AsyncClient<byte[]>(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient, ClientConfig);
        }

        /// <inheritdoc />
        public override IAsyncClient<byte[]> Create(ITcpClient tcpClient)
        {
            var writer = new NetworkMessageWriter(tcpClient);
            var reader = new NetworkMessageReader(tcpClient, int.Parse(ClientConfig.GetProperty("MaxMessageSize"))); 
            var inboundSpooler = InboundMessageSpooler<byte[]>.Create(reader); 
            var outboundSpooler = OutboundMessageSpooler<byte[]>.Create(writer);
            var messagePoller = MessagePoller<byte[]>.Create(inboundSpooler.Queue);
            var messageFactory = new OutboundMessageFactory<byte[]>();

            return new AsyncMessagingClient(inboundSpooler, outboundSpooler, messagePoller, messageFactory, tcpClient, ClientConfig);
        }

        /// <inheritdoc />
        public override IAsyncClient<byte[]> Create(IPEndPoint remoteEndPoint)
        {
            var tcpClient = new BaseTcpClient(new TcpClient(remoteEndPoint.Address.ToString(), remoteEndPoint.Port));
            return Create(tcpClient);
        }

        /// <inheritdoc />
        public override IAsyncClient<byte[]> Create()
        {
            var tcpClient = new BaseTcpClient(new TcpClient());
            return Create(tcpClient);
        }
    }
}
