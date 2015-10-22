using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    /// <summary>
    /// Runs a thread that listens for incoming connections and creates instances of AsyncClient using the given AsyncClientFactory
    /// </summary>
    /// <typeparam name="T">Type for the message object associated with the protocol that the AsyncClient instance is using.</typeparam>
    public class ClientConnectionAgent<T> : ThreadRunner, IClientConnectionAgent<T>
    {
        public event NewClientConnected<T> OnNewClientConnection;
        private IClientConnectionAgentRunnable<T> runnable;

        public ClientConnectionAgent(IClientConnectionAgentRunnable<T> runnable) : base(runnable)
        {
            this.runnable = runnable;
            runnable.OnNewClientConnection += runnable_OnNewClientConnection;
        }

        private void runnable_OnNewClientConnection(object sender, NewClientConnectedEventArgs<T> e)
        {
            var onNewClientConnection = OnNewClientConnection;
            if(onNewClientConnection != null)
            {
                onNewClientConnection(this, e);
            }
        }

        /// <summary>
        /// Creates an instance of this class using the given AsyncClientFactory and wrapped TcpListener
        /// </summary>
        /// <param name="clientFactory">Instance of AsyncClientFactory to use to create AsyncClient instances per new connection accepted.</param>
        /// <param name="listener">The TcpListener instance to use.</param>
        /// <returns></returns>
        public static ClientConnectionAgent<T> Create(AsyncClientFactory<T> clientFactory, ITcpListener listener)
        {
            ClientConnectionAgentRunnable<T> runnable = new ClientConnectionAgentRunnable<T>(listener, clientFactory);
            return new ClientConnectionAgent<T>(runnable);
        }

        /// <summary>
        /// Starts listening for new connections.
        /// </summary>
        public override void Start()
        {
            runnable.TcpListener.Start();
            base.Start();
        }
    }
}
