using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public interface IAsyncClientFactory<T>
    {
        IAsyncClient<T> Create(IInboundMessageSpooler<T> inboundSpooler, IOutboundMessageSpooler<T> outboundSpooler, IMessagePoller<T> messagePoller, IOutboundMessageFactory<T> messageFactory, ITcpClient tcpClient);
        IAsyncClient<T> Create(ITcpClient tcpClient);
        IAsyncClient<T> Create(IPEndPoint remoteEndPoint);
        IAsyncClient<T> Create();

        ClientConfig ClientConfig { get; }
    }
}
