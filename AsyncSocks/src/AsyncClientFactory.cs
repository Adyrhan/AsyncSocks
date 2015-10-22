using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Base class to use for the implementation of an AsyncClientFactory. It creates instances of AsyncClient. <see cref="AsyncClient{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the messages associated with the protocol that the created AsyncClient instances will use.</typeparam>
    public abstract class AsyncClientFactory<T> : IAsyncClientFactory<T>
    {
        /// <summary>
        /// Takes a ClientConfig object that will be used to create AsyncClient instances.
        /// </summary>
        /// <param name="clientConfig">The instance of a ClientConfig object that will be used to create AsyncClient instances.</param>
        public AsyncClientFactory(ClientConfig clientConfig)
        {
            ClientConfig = clientConfig;
        }

        // FIXME: This version of the constructor doesn't belong to the abstract class
        public AsyncClientFactory()
        {
            ClientConfig = AsyncMessagingClientConfig.GetDefault();
        }

        /// <summary>
        /// Returns the ClientConfig instance used by this factory.
        /// </summary>
        public ClientConfig ClientConfig { get; }

        /// <summary>
        /// This version of the Create method is used mostly by tests. Use one of the other versions of this method. See <see cref="Create()"/>, <see cref="Create(IPEndPoint)"/>, or <see cref="Create(ITcpClient)"/>.
        /// </summary>
        /// <param name="inboundSpooler"></param>
        /// <param name="outboundSpooler"></param>
        /// <param name="messagePoller"></param>
        /// <param name="messageFactory"></param>
        /// <param name="tcpClient"></param>
        /// <returns>An instance of an implementation of AsyncClient</returns>
        public abstract IAsyncClient<T> Create(IInboundMessageSpooler<T> inboundSpooler, IOutboundMessageSpooler<T> outboundSpooler, IMessagePoller<T> messagePoller, IOutboundMessageFactory<T> messageFactory, ITcpClient tcpClient);

        /// <summary>
        /// Creates an AsyncClient instance from an ITcpClient instance.
        /// </summary>
        /// <param name="tcpClient">ITcpClient instance configured to connect to a remote end point.</param>
        /// <returns>An instance of an implementation of AsyncClient</returns>
        public abstract IAsyncClient<T> Create(ITcpClient tcpClient);

        /// <summary>
        /// Creates an AsyncClient instance that will connect to the specified remote end point.
        /// </summary>
        /// <param name="remoteEndPoint">Remote end point to connect to.</param>
        /// <returns>An instance of an implementation of AsyncClient</returns>
        public abstract IAsyncClient<T> Create(IPEndPoint remoteEndPoint);

        /// <summary>
        /// Creates an AsyncClient instance that won't be connected. You must call AsyncClient.Connect(IPEndPoint) to connect it to a remote end point.
        /// </summary>
        /// <returns>An instance of an implementation of AsyncClient</returns>
        public abstract IAsyncClient<T> Create();

    }
}
