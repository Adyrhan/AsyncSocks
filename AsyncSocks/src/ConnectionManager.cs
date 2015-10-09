using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public class ConnectionManager<T> : IConnectionManager<T>
    {
        private Dictionary<IPEndPoint, IAsyncClient<T>> dict;

        public event NewMessageReceived<T> OnNewMessageReceived;
        public event PeerDisconnected<T> OnPeerDisconnected;

        public ConnectionManager(Dictionary<IPEndPoint, IAsyncClient<T>> dict)
        {
            this.dict = dict;
        }

        public void CloseAllConnections()
        {
            var dictCopy = new Dictionary<IPEndPoint, IAsyncClient<T>>(dict);
            foreach (KeyValuePair<IPEndPoint, IAsyncClient<T>> entry in dictCopy)
            {
                entry.Value.Close();
            }
            
        }

        public void Add(IAsyncClient<T> peerConnection)
        {
            dict[(IPEndPoint) peerConnection.RemoteEndPoint] = peerConnection;
            peerConnection.OnNewMessageReceived += peerConnection_OnNewMessageReceived;
            peerConnection.OnPeerDisconnected += PeerConnection_OnPeerDisconnected;
            peerConnection.Start();
            
        }

        private void PeerConnection_OnPeerDisconnected(object sender, PeerDisconnectedEventArgs<T> e)
        {
            dict.Remove((IPEndPoint)e.Peer.RemoteEndPoint);

            var onPeerDisconnected = OnPeerDisconnected;

            if (onPeerDisconnected != null)
            {
                onPeerDisconnected(this, e);
            }
        }

        void peerConnection_OnNewMessageReceived(object sender, NewMessageReceivedEventArgs<T> e)
        {
            if (OnNewMessageReceived != null)
            {
                OnNewMessageReceived(this, e);
            }
        }
    }
}
