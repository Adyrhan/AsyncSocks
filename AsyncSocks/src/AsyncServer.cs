using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public class AsyncServer<T> : IAsyncServer<T>
    {
        private IClientConnectionAgent<T> clientConnectionAgent;
        private IConnectionManager<T> connectionManager;
        private ITcpListener tcpListener;        

        public IConnectionManager<T> ConnectionManager
        {
            get
            {
                return connectionManager;
            }
        }

        public ClientConfig ClientConfig { get; }

        public event NewMessageReceived<T> OnNewMessageReceived;
        public event NewClientConnected<T> OnNewClientConnected;
        public event PeerDisconnected<T> OnPeerDisconnected;

        public AsyncServer(IClientConnectionAgent<T> clientConnectionAgent, IConnectionManager<T> connectionManager, ITcpListener tcpListener, ClientConfig clientConfig)
        {
            ClientConfig = clientConfig;
            this.clientConnectionAgent = clientConnectionAgent;
            this.connectionManager = connectionManager;
            this.tcpListener = tcpListener;
            this.connectionManager.OnNewMessageReceived += connectionManager_OnNewClientMessageReceived;
            this.connectionManager.OnPeerDisconnected += ConnectionManager_OnPeerDisconnected;
            this.clientConnectionAgent.OnNewClientConnection += clientConnectionAgent_OnNewClientConnection;
        }

        private void ConnectionManager_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs<T> e)
        {
            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this, e);
            }
        }

        void clientConnectionAgent_OnNewClientConnection(object sender, NewClientConnectedEventArgs<T> e)
        {
            connectionManager.Add(e.Client);
            if (OnNewClientConnected != null) 
            {
                OnNewClientConnected(this, e);
            }
        }

        private void connectionManager_OnNewClientMessageReceived(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewMessageReceived != null)
            {
                OnNewMessageReceived(this, e);
            }
        }

        public void Start()
        {
            clientConnectionAgent.Start();
        }

        public void Stop()
        {
            clientConnectionAgent.Stop();
            connectionManager.CloseAllConnections();
        }
    }
}
