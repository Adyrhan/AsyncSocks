using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public abstract class AsyncClientFactory<T> : IAsyncClientFactory<T>
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

        public abstract IAsyncClient<T> Create(IInboundMessageSpooler<T> inboundSpooler, IOutboundMessageSpooler<T> outboundSpooler, IMessagePoller<T> messagePoller, IOutboundMessageFactory<T> messageFactory, ITcpClient tcpClient);
        public abstract IAsyncClient<T> Create(ITcpClient tcpClient);
        public abstract IAsyncClient<T> Create(IPEndPoint remoteEndPoint);
        public abstract IAsyncClient<T> Create();

    }
}
