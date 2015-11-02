using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    /// <summary>
    /// Holds the client objects and notifies the server instance of messages sent by the clients and client disconnections.
    /// </summary>
    /// <typeparam name="T">Type for the message object associated with the protocol that the AsyncClient instance is using.</typeparam>
    public class ConnectionManager<T> : IConnectionManager<T>
    {
        private Dictionary<IPEndPoint, IAsyncClient<T>> dict;

        public event NewMessageReceived<T> OnNewMessageReceived;
        public event PeerDisconnected<T> OnPeerDisconnected;

        public ConnectionManager(Dictionary<IPEndPoint, IAsyncClient<T>> dict)
        {
            this.dict = dict;
        }

        /// <summary>
        /// Closes all connections.
        /// </summary>
        public void CloseAllConnections()
        {
            var dictCopy = new Dictionary<IPEndPoint, IAsyncClient<T>>(dict);
            foreach (KeyValuePair<IPEndPoint, IAsyncClient<T>> entry in dictCopy)
            {
                entry.Value.Close();
            }
            
        }

        /// <summary>
        /// Adds a new client connection to the manager instance.
        /// </summary>
        /// <param name="peerConnection">Instance of AsyncClient</param>
        public void Add(IAsyncClient<T> peerConnection)
        {
            dict[(IPEndPoint) peerConnection.RemoteEndPoint] = peerConnection;
            peerConnection.OnNewMessage += peerConnection_OnNewMessageReceived;
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
