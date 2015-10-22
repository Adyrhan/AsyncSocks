using System.Collections.Concurrent;


namespace AsyncSocks
{
    /// <summary>
    /// Runs a thread that reads messages comming from a client connection and stores them in a queue.
    /// </summary>
    /// <typeparam name="T">Type for the message object associated with the protocol that the AsyncClient instance is using.</typeparam>
    public class InboundMessageSpooler<T> : ThreadRunner, IInboundMessageSpooler<T>
    {
        /// <summary>
        /// This event fires when the client has closed the connection.
        /// </summary>
        public event PeerDisconnected<T> OnPeerDisconnected;

        /// <summary>
        /// Static factory to create instances of InboundMessageSpooler from a INetworkReader instance
        /// </summary>
        /// <param name="reader">Instance that is responsible to read messages from the network.</param>
        /// <returns></returns>
        public static InboundMessageSpooler<T> Create(INetworkReader<T> reader)
        {
            var queue = new BlockingCollection<ReadResult<T>>(new ConcurrentQueue<ReadResult<T>>());
            var runnable = new InboundMessageSpoolerRunnable<T>(reader, queue);
            var spooler = new InboundMessageSpooler<T>(runnable);

            return spooler;
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

        /// <summary>
        /// Returns the queue used to store the incoming messages
        /// </summary>
        public BlockingCollection<ReadResult<T>> Queue
        {
            get { return ((IInboundMessageSpoolerRunnable<T>) Runnable).Queue; }
        }

    }
}
