using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Root namespace. Most classes that allow for implementations are in this namespace.
    /// </summary>
    class NamespaceDoc { }

    /// <summary>
    /// Class that represents the connection to a host. It sends and receives messages of type T.
    /// <para>To create an instance of this class, use the corresponding AsyncClientFactory instance for the subclass that you want to instantiate.<see cref="AsyncClientFactory{T}"/></para>
    /// </summary>
    /// <typeparam name="T">The type associated with the protocol that this instance will use to read and write messages.</typeparam>
    public class AsyncClient<T> : IAsyncClient<T>
    {
        private IInboundMessageSpooler<T> inboundSpooler;
        private IOutboundMessageSpooler<T> outboundSpooler;
        private ITcpClient tcpClient;
        private IMessagePoller<T> poller;
        private IOutboundMessageFactory<T> messageFactory;
        private bool isClosing;

        /// <summary>
        /// Event fires for each message received from this client.
        /// </summary>
        public event NewMessageReceived<T> OnNewMessage;

        /// <summary>
        /// Event fires when this client closes connection.
        /// </summary>
        public event PeerDisconnected<T> OnPeerDisconnected;

        /// <summary>
        /// Fires whenever a read error happened.
        /// </summary>
        public event ReadErrorEventHandler OnReadError;

        public AsyncClient(IInboundMessageSpooler<T> inboundSpooler, IOutboundMessageSpooler<T> outboundSpooler, IMessagePoller<T> poller, IOutboundMessageFactory<T> messageFactory, ITcpClient tcpClient, ClientConfig clientConfig)
        {
            this.inboundSpooler = inboundSpooler;
            this.outboundSpooler = outboundSpooler;
            this.poller = poller;
            this.tcpClient = tcpClient;
            this.messageFactory = messageFactory;
            ClientConfig = clientConfig;

            setupEvents();
            if (tcpClient.Connected) saveEndPoints();
        }

        private void setupEvents()
        {
            poller.OnNewClientMessageReceived += RaiseOnNewMessage;
            inboundSpooler.OnPeerDisconnected += RaiseOnPeerDisconnected;
            poller.OnReadError += RaiseOnReadError;
        }

        private void saveEndPoints()
        {
            LocalEndPoint = tcpClient.Socket.LocalEndPoint;
            RemoteEndPoint = tcpClient.Socket.RemoteEndPoint;
        }

        protected virtual void RaiseOnReadError(object sender, ReadErrorEventArgs e)
        {
            var onReadError = OnReadError;
            if (onReadError != null)
            {
                onReadError(this, e);
            }
        }

        protected virtual void RaiseOnPeerDisconnected(object sender, PeerDisconnectedEventArgs<T> e)
        {
            var onPeerDisconnected = OnPeerDisconnected;
            if (onPeerDisconnected != null)
            {
                var ev = new PeerDisconnectedEventArgs<T>(this);
                onPeerDisconnected(this, ev);
            }

            Disconnect();
        }

        protected void Disconnect()
        {
            //Task.Run(() =>
            //{
                Close();
            //});
        }

        protected virtual void RaiseOnNewMessage(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewMessage != null)
            {
                var newE = new NewMessageReceivedEventArgs<T>(this, e.Message);
                OnNewMessage(this, newE);
            }
        }

        /// <summary>
        /// Sends a message to this client without confirmation callback.
        /// </summary>
        /// <param name="message">The message object to send.</param>
        public void SendMessage(T message)
        {
            SendMessage(message, null);
        }

        /// <summary>
        /// Sends a message to this client. The callback is a delegate used to notify of the completion of the message sending. 
        /// Any exception that occurred in the process would be
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public virtual void SendMessage(T message, Action<bool, SocketException> callback)
        {
            OutboundMessage<T> msg = messageFactory.Create(message, callback);
            outboundSpooler.Enqueue(msg);
        }

        /// <summary>
        /// Starts this client's threads for receiving and sending messages.
        /// </summary>
        public void Start()
        {
            inboundSpooler.Start();
            outboundSpooler.Start();
            poller.Start();
        }

        /// <summary>
        /// Stops receiving and sending messages and closes the connection.
        /// </summary>
        public void Close()
        {
            if (!isClosing)
            {
                isClosing = true;
                tcpClient.Close();
                poller.Stop();
                inboundSpooler.Stop();
                outboundSpooler.Stop();
            }
        }

        /// <summary>
        /// Returns the remote end point information
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; }
        
        /// <summary>
        /// Returns the local end point information
        /// </summary>
        public EndPoint LocalEndPoint { get; private set; }

        /// <summary>
        /// Indicates if this client instance is listening for incoming messages and ready to send them.
        /// </summary>
        /// <returns>True if active, false otherwise.</returns>
        public bool IsActive()
        {
            return (inboundSpooler.IsRunning() && outboundSpooler.IsRunning() && poller.IsRunning());
        }

        /// <summary>
        /// Returns the underlying wrapped TcpClient instance
        /// </summary>
        public ITcpClient TcpClient
        {
            get { return tcpClient; }
        }

        /// <summary>
        /// This property returns the ClientConfig object used to create this instance.
        /// </summary>
        public ClientConfig ClientConfig { get; }

        /// <summary>
        /// Connects to the remote host that was previously configured and calls Start(). <see cref="Start"/>
        /// </summary>
        public void Connect()
        {
            Connect(null);
            Start();
        }

        /// <summary>
        /// Connects to the specified remote end point and calls start.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point for the host.</param>
        public void Connect(IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
            {
                tcpClient.Connect();
            }
            else
            {
                tcpClient.Connect(remoteEndPoint);
            }
            
            saveEndPoints();
            Start();
        }
        
    }
}
