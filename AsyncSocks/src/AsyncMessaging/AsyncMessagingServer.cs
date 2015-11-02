using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocks.AsyncMessaging
{
    /// <summary>
    /// This creates a server for receiving and sending binary messages.
    /// </summary>
    public class AsyncMessagingServer : AsyncServer<byte[]>
    {
        public AsyncMessagingServer
        (
            IClientConnectionAgent<byte[]> clientConnectionAgent, IConnectionManager<byte[]> connectionManager, ITcpListener tcpListener, ClientConfig clientConfig
        ) : base(clientConnectionAgent, connectionManager, tcpListener, clientConfig) { }

        /// <summary>
        /// This will create a server that will bind to the local end point specified. It will use the default AsyncMessagingClientConfig for the factory to create instances of AsyncMessagingClient per connection accepted.
        /// </summary>
        /// <param name="localEndPoint">The local end point to bind to.</param>
        /// <returns>Instance of AsyncMessagingServer</returns>
        public static AsyncMessagingServer Create(IPEndPoint localEndPoint)
        {
            var clientConfig = AsyncMessagingClientConfig.GetDefault();
            return Create(localEndPoint, clientConfig);
        }

        /// <summary>
        /// This will create a server that will bind to the local end point specified. It will used the specified ClientConfig instance for the factory to create instances of AsyncMessagingClient per connection accepted.
        /// </summary>
        /// <param name="localEndPoint">The local end point to bind to.</param>
        /// <param name="clientConfig">The ClientConfig instance to use.</param>
        /// <returns></returns>
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
