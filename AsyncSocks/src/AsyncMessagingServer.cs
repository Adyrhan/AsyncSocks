using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocks
{
    public class AsyncMessagingServer : AsyncServer<byte[]>
    {
        public AsyncMessagingServer
        (
            IClientConnectionAgent<byte[]> clientConnectionAgent, IConnectionManager<byte[]> connectionManager, ITcpListener tcpListener, ClientConfig clientConfig
        ) : base(clientConnectionAgent, connectionManager, tcpListener, clientConfig) { }

        public static AsyncMessagingServer Create(IPEndPoint localEndPoint)
        {
            var clientConfig = AsyncMessagingClientConfig.GetDefault();
            return Create(localEndPoint, clientConfig);
        }

        public static AsyncMessagingServer Create(IPEndPoint localEndPoint, ClientConfig clientConfig)
        {
            var tcpListener = new BaseTcpListener(new TcpListener(localEndPoint));
            var factory = new AsyncMessagingClientFactory(clientConfig);
            var clientConnectionAgent = ClientConnectionAgent<byte[]>.Create(factory, tcpListener);
            var connectionManager = new ConnectionManager<byte[]>(new Dictionary<IPEndPoint, IAsyncClient<byte[]>>());
            return new AsyncMessagingServer(clientConnectionAgent, connectionManager, tcpListener, clientConfig);
        }
    }
}
