namespace AsyncSocks
{
    /// <summary>
    /// This class represents a server. It will handle messages of type T.
    /// <para>To create an instance, use the corresponding factories for the subclass that you want to use. 
    /// <see cref="AsyncMessagingServer.Create(System.Net.IPEndPoint)"/> for how to create an instance of AsyncMessagingServer subclass.</para>
    /// </summary>
    /// <typeparam name="T">The type associated with the protocol that this instance will use to read and write messages.</typeparam>
    public class AsyncServer<T> : IAsyncServer<T>
    {
        private IClientConnectionAgent<T> clientConnectionAgent;
        private IConnectionManager<T> connectionManager;
        private ITcpListener tcpListener;        

        public IConnectionManager<T> ConnectionManager
        {
            get
            {
                return connectionManager;
            }
        }

        /// <summary>
        /// Returns the ClientConfig instance used by this server. Every new AsyncClient instance created by this server will be 
        /// setup according to the values of this ClientConfig instance.
        /// </summary>
        public ClientConfig ClientConfig { get; }

        /// <summary>
        /// This event will be fired for every message received from any of the connections accepted by this server instance.
        /// </summary>
        public event NewMessageReceived<T> OnNewMessageReceived;

        /// <summary>
        /// Fires for every new client connected.
        /// </summary>
        public event NewClientConnected<T> OnNewClientConnected;

        /// <summary>
        /// Fires for every client that disconnects from the server.
        /// </summary>
        public event PeerDisconnected<T> OnPeerDisconnected;

        public AsyncServer(IClientConnectionAgent<T> clientConnectionAgent, IConnectionManager<T> connectionManager, ITcpListener tcpListener, ClientConfig clientConfig)
        {
            ClientConfig = clientConfig;
            this.clientConnectionAgent = clientConnectionAgent;
            this.connectionManager = connectionManager;
            this.tcpListener = tcpListener;
            this.connectionManager.OnNewMessageReceived += connectionManager_OnNewClientMessageReceived;
            this.connectionManager.OnPeerDisconnected += ConnectionManager_OnPeerDisconnected;
            this.clientConnectionAgent.OnNewClientConnection += clientConnectionAgent_OnNewClientConnection;
        }

        private void ConnectionManager_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs<T> e)
        {
            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this, e);
            }
        }

        void clientConnectionAgent_OnNewClientConnection(object sender, NewClientConnectedEventArgs<T> e)
        {
            connectionManager.Add(e.Client);
            if (OnNewClientConnected != null) 
            {
                OnNewClientConnected(this, e);
            }
        }

        private void connectionManager_OnNewClientMessageReceived(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewMessageReceived != null)
            {
                OnNewMessageReceived(this, e);
            }
        }

        /// <summary>
        /// Starts listening for incoming connections.
        /// </summary>
        public void Start()
        {
            clientConnectionAgent.Start();
        }

        /// <summary>
        /// Stops listening for incoming connections and closes all connections associated to this server instance.
        /// </summary>
        public void Stop()
        {
            clientConnectionAgent.Stop();
            connectionManager.CloseAllConnections();
        }
    }
}
