using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public class ConnectionManager : IConnectionManager
    {
        private Dictionary<IPEndPoint, IAsyncClient> dict;

        public event NewMessageReceived OnNewMessageReceived;
        public event PeerDisconnected OnPeerDisconnected;

        public ConnectionManager(Dictionary<IPEndPoint, IAsyncClient> dict)
        {
            this.dict = dict;
        }

        public void CloseAllConnetions()
        {
            foreach(KeyValuePair<IPEndPoint, IAsyncClient> entry in dict)
            {
                entry.Value.Close();
            }
            dict.Clear();
        }

        public void Add(IAsyncClient peerConnection)
        {
            dict[(IPEndPoint) peerConnection.RemoteEndPoint] = peerConnection;
            peerConnection.OnNewMessageReceived += peerConnection_OnNewMessageReceived;
            peerConnection.OnPeerDisconnected += PeerConnection_OnPeerDisconnected;
            peerConnection.Start();
            
        }

        private void PeerConnection_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs e)
        {
            dict.Remove((IPEndPoint)e.Peer.RemoteEndPoint);

            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this, e);
            }
        }

        void peerConnection_OnNewMessageReceived(object sender, NewMessageReceivedEventArgs e)
        {
            if (OnNewMessageReceived != null)
            {
                OnNewMessageReceived(this, e);
            }
        }
    }
}
