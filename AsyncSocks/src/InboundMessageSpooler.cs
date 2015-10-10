using System;
using System.Collections.Concurrent;


namespace AsyncSocks
{
    public class InboundMessageSpooler<T> : ThreadRunner, IInboundMessageSpooler<T>
    {
        public event PeerDisconnected<T> OnPeerDisconnected;

        public static InboundMessageSpooler<T> Create(INetworkReader<T> reader, int maxMessageSize)
        {
            var queue = new BlockingCollection<ReadResult<T>>(new ConcurrentQueue<ReadResult<T>>());
            var runnable = new InboundMessageSpoolerRunnable<T>(reader, queue);
            var spooler = new InboundMessageSpooler<T>(runnable);

            return spooler;
        }

        public static InboundMessageSpooler<T> Create(INetworkReader<T> reader)
        {
            return Create(reader, ClientConfig.GetDefault().MaxMessageSize);
        }

        public InboundMessageSpooler(IInboundMessageSpoolerRunnable<T> runnable) : base(runnable)
        {
            runnable.OnPeerDisconnected += Runnable_OnPeerDisconnected;
        }

        private void Runnable_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs<T> e)
        {
            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this, e);
            }
        }

        public BlockingCollection<ReadResult<T>> Queue
        {
            get { return ((IInboundMessageSpoolerRunnable<T>) Runnable).Queue; }
        }

    }
}
