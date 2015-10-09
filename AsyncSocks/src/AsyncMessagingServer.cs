using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            var clientConfig = ClientConfig.GetDefault();
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
