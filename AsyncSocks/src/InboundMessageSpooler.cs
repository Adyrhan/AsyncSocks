using System;
using System.Collections.Concurrent;


namespace AsyncSocks
{
    public class InboundMessageSpooler : ThreadRunner, IInboundMessageSpooler
    {
        public event PeerDisconnected OnPeerDisconnected;

        public static InboundMessageSpooler Create(ITcpClient tcpClient, int maxMessageSize)
        {
            var reader = new NetworkMessageReader(tcpClient, maxMessageSize);
            var queue = new BlockingCollection<NetworkMessage>(new ConcurrentQueue<NetworkMessage>());
            var runnable = new InboundMessageSpoolerRunnable(reader, queue);
            var spooler = new InboundMessageSpooler(runnable);

            //spooler.ThreadName = "InboundMessageSpooler " + tcpClient.Socket.LocalEndPoint;

            return spooler;
        }

        public static InboundMessageSpooler Create(ITcpClient tcpClient)
        {
            return Create(tcpClient, ClientConfig.GetDefault().MaxMessageSize);
        }

        public InboundMessageSpooler(IInboundMessageSpoolerRunnable runnable) : base(runnable)
        {
            runnable.OnPeerDisconnected += Runnable_OnPeerDisconnected;
        }

        private void Runnable_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this, e);
            }

        }

        public BlockingCollection<NetworkMessage> Queue
        {
            get { return ((IInboundMessageSpoolerRunnable) Runnable).Queue; }
        }

    }
}
