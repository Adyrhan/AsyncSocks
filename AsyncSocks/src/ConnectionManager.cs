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
        private IMessagePoller poller;

        public event NewClientMessageDelegate OnNewClientMessageReceived;

        public ConnectionManager(Dictionary<IPEndPoint, IPeerConnection> dict, IMessagePoller poller)
        {
            this.dict = dict;
            this.poller = poller;
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
            peerConnection.Start();
        }
    }
}
