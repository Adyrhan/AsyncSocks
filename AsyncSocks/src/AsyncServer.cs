using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public delegate void NewPeerConnectionDelegate(IPeerConnection client);
    public class AsyncServer : IAsyncServer
    {
        private IClientConnectionAgent clientConnectionAgent;
        private IConnectionManager connectionManager;
        private ITcpListener tcpListener;

        public event NewClientMessageReceived OnNewMessageReceived;
        public event NewPeerConnectionDelegate OnNewClientConnected;

        public AsyncServer(IClientConnectionAgent clientConnectionAgent, IConnectionManager connectionManager, ITcpListener tcpListener)
        {
            // TODO: Complete member initialization
            this.clientConnectionAgent = clientConnectionAgent;
            this.connectionManager = connectionManager;
            this.tcpListener = tcpListener;
            this.connectionManager.OnNewClientMessageReceived += connectionManager_OnNewClientMessageReceived;

            this.clientConnectionAgent.OnNewClientConnection += clientConnectionAgent_OnNewClientConnection;
        }

        void clientConnectionAgent_OnNewClientConnection(IPeerConnection client)
        {
            connectionManager.Add(client);
            if (OnNewClientConnected != null) 
            {
                OnNewClientConnected(client);
            }
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

        public static AsyncServer Create(IPEndPoint localEndPoint)
        {
            BaseTcpListener tcpListener = new BaseTcpListener(new TcpListener(localEndPoint));
            ClientConnectionAgent clientConnectionAgent = ClientConnectionAgent.Create(tcpListener);
            ConnectionManager connectionManager = new ConnectionManager(new Dictionary<IPEndPoint, IPeerConnection>());
            return new AsyncServer(clientConnectionAgent, connectionManager, tcpListener);
        }
    }
}
