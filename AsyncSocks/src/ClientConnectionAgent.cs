using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class ClientConnectionAgent<T> : ThreadRunner, IClientConnectionAgent<T>
    {
        public event NewClientConnected<T> OnNewClientConnection;
        private IClientConnectionAgentRunnable<T> runnable;

        public ClientConnectionAgent(IClientConnectionAgentRunnable<T> runnable) : base(runnable)
        {
            // TODO: Complete member initialization
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

        public static ClientConnectionAgent<T> Create(AsyncClientFactory<T> clientFactory, ITcpListener listener)
        {
            ClientConnectionAgentRunnable<T> runnable = new ClientConnectionAgentRunnable<T>(listener, clientFactory);
            return new ClientConnectionAgent<T>(runnable);
        }

        public override void Start()
        {
            runnable.TcpListener.Start();
            base.Start();
        }
    }
}
