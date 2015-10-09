﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            var reader = new NetworkMessageReader(tcpClient, ClientConfig.MaxMessageSize); // FIXME: This ClientConfig object here is quite coupled if we plan to use it as a configuration object for every kind of implementation. It should be abstract, or just an interface
            var inboundSpooler = InboundMessageSpooler<byte[]>.Create(reader, tcpClient, ClientConfig.MaxMessageSize); // FIXME: This last parameter is meant to be used for a specific implementation of INetworkReader. It wouldn't be right to use it in the generic implementation of the AsyncClientFactory
            var outboundSpooler = OutboundMessageSpooler<byte[]>.Create(tcpClient);
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
