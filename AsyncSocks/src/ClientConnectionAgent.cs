using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class ClientConnectionAgent : ThreadRunner, IClientConnectionAgent
    {
        public event NewClientConnectionDelegate OnNewClientConnection;
        private IClientConnectionAgentRunnable runnable;

        public ClientConnectionAgent(IClientConnectionAgentRunnable runnable) : base(runnable)
        {
            // TODO: Complete member initialization
            this.runnable = runnable;
            runnable.OnNewClientConnection += runnable_OnNewClientConnection;
        }

        private void runnable_OnNewClientConnection(ITcpClient client)
        {
            OnNewClientConnection(client);
        }
    }
}
