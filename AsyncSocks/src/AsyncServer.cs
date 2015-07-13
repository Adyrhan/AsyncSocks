using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class AsyncServer
    {
        private IClientConnectionAgent clientConnectionAgent;
        private IConnectionManager connectionManager;
        private ITcpListener tcpListener;

        public event NewClientMessageDelegate OnNewMessageReceived;

        public AsyncServer(IClientConnectionAgent clientConnectionAgent, IConnectionManager connectionManager, ITcpListener tcpListener)
        {
            // TODO: Complete member initialization
            this.clientConnectionAgent = clientConnectionAgent;
            this.connectionManager = connectionManager;
            this.tcpListener = tcpListener;
            this.connectionManager.OnNewClientMessageReceived += connectionManager_OnNewClientMessageReceived;
        }

        private void connectionManager_OnNewClientMessageReceived(IPeerConnection sender, byte[] message)
        {
            if (OnNewMessageReceived != null)
            {
                OnNewMessageReceived(sender, message);
            }
        }

        public void Start()
        {
            clientConnectionAgent.Start();
        }

        public void Stop()
        {
            clientConnectionAgent.Stop();
            connectionManager.CloseAllConnetions();
        }
    }
}
