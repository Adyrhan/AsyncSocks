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

        private void runnable_OnNewClientConnection(IAsyncClient client)
        {
            OnNewClientConnection(client);
        }

        public static ClientConnectionAgent Create(ITcpListener listener)
        {
            AsyncClientFactory factory = new AsyncClientFactory();
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
