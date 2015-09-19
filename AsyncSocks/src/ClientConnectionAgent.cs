using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class ClientConnectionAgent : ThreadRunner, IClientConnectionAgent
    {
        public event NewClientConnected OnNewClientConnection;
        private IClientConnectionAgentRunnable runnable;

        public ClientConnectionAgent(IClientConnectionAgentRunnable runnable) : base(runnable)
        {
            // TODO: Complete member initialization
            this.runnable = runnable;
            runnable.OnNewClientConnection += runnable_OnNewClientConnection;
        }

        private void runnable_OnNewClientConnection(object sender, NewClientConnectedEventArgs e)
        {
            var onNewClientConnection = OnNewClientConnection;
            if(onNewClientConnection != null)
            {
                onNewClientConnection(this, e);
            }
        }

        public static ClientConnectionAgent Create(ITcpListener listener, ClientConfig clientConfig)
        {
            AsyncClientFactory factory = new AsyncClientFactory(clientConfig);
            ClientConnectionAgentRunnable runnable = new ClientConnectionAgentRunnable(listener, factory);
            return new ClientConnectionAgent(runnable);
        }

        public override void Start()
        {
            runnable.TcpListener.Start();
            base.Start();
        }
    }
}
