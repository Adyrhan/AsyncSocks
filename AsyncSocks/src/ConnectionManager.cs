using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public class ConnectionManager : IConnectionManager
    {
        private Dictionary<IPEndPoint, IPeerConnection> dict;

        public event NewClientMessageReceived OnNewClientMessageReceived;

        public ConnectionManager(Dictionary<IPEndPoint, IPeerConnection> dict)
        {
            this.dict = dict;
        }

        public void CloseAllConnetions()
        {
            foreach(KeyValuePair<IPEndPoint, IPeerConnection> entry in dict)
            {
                entry.Value.Close();
            }
            dict.Clear();
        }

        public void Add(IPeerConnection peerConnection)
        {
            dict[(IPEndPoint) peerConnection.RemoteEndPoint] = peerConnection;
            peerConnection.OnNewMessageReceived += peerConnection_OnNewMessageReceived;
            peerConnection.Start();
            
        }

        void peerConnection_OnNewMessageReceived(IPeerConnection sender, byte[] message)
        {
            if (OnNewClientMessageReceived != null)
            {
                OnNewClientMessageReceived(sender, message);
            }
        }
    }
}
