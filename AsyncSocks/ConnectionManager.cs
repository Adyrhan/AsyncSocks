using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public class ConnectionManager
    {
        private Dictionary<IPEndPoint, PeerConnection> dict;

        public void Add(ITcpClient tcpClient)
        {
            var inboundSpooler = InboundMessageSpooler.Create(tcpClient);
            var outboundSpooler = OutboundMessageSpooler.Create(tcpClient);
            var connection = new PeerConnection(inboundSpooler, outboundSpooler, tcpClient);

            dict[(IPEndPoint) tcpClient.Client.RemoteEndPoint] = connection;
        }

        public ConnectionManager(Dictionary<IPEndPoint, PeerConnection> dict)
        {
            this.dict = dict;
        }
    }
}
