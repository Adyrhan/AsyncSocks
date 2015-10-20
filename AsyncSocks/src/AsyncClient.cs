using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class AsyncClient<T> : IAsyncClient<T>
    {
        private IInboundMessageSpooler<T> inboundSpooler;
        private IOutboundMessageSpooler<T> outboundSpooler;
        private ITcpClient tcpClient;
        private IMessagePoller<T> poller;
        private IOutboundMessageFactory<T> messageFactory;
        private bool isClosing;

        public event NewMessageReceived<T> OnNewMessageReceived;
        public event PeerDisconnected<T> OnPeerDisconnected;

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
            poller.OnNewClientMessageReceived += poller_OnNewClientMessageReceived;
            inboundSpooler.OnPeerDisconnected += InboundSpooler_OnPeerDisconnected;
        }

        private void saveEndPoints()
        {
            LocalEndPoint = tcpClient.Socket.LocalEndPoint;
            RemoteEndPoint = tcpClient.Socket.RemoteEndPoint;
        }

        private void InboundSpooler_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs<T> e)
        {
            Task.Run(() =>
            {
                var onPeerDisconnected = OnPeerDisconnected;
                if (onPeerDisconnected != null)
                {
                    var ev = new PeerDisconnectedEventArgs<T>(this);
                    onPeerDisconnected(this, ev);
                }

                if (!isClosing) Close();
            });
            
        }

        private void poller_OnNewClientMessageReceived(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewMessageReceived != null)
            {
                var newE = new NewMessageReceivedEventArgs<T>(this, e.Message);
                OnNewMessageReceived(this, newE);
            }
        }

        public void SendMessage(T message)
        {
            SendMessage(message, null);
        }

        public virtual void SendMessage(T message, Action<bool, SocketException> callback)
        {
            OutboundMessage<T> msg = messageFactory.Create(message, callback);
            outboundSpooler.Enqueue(msg);
        }

        public void Start()
        {
            inboundSpooler.Start();
            outboundSpooler.Start();
            poller.Start();
        }

        public void Close()
        {
            isClosing = true;
            tcpClient.Close();
            poller.Stop();
            inboundSpooler.Stop();
            outboundSpooler.Stop();
        }

        public EndPoint RemoteEndPoint { get; private set; }
        
        public EndPoint LocalEndPoint { get; private set; }

        public bool IsActive()
        {
            return (inboundSpooler.IsRunning() && outboundSpooler.IsRunning() && poller.IsRunning());
        }

        public ITcpClient TcpClient
        {
            get { return tcpClient; }
        }

        public ClientConfig ClientConfig { get; }

        public void Connect()
        {
            Connect(null);
            Start();
        }

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
