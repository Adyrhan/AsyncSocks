using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public class ConnectionManager
    {
        private Dictionary<IPEndPoint, IPeerConnection> dict;
        private IPeerConnectionFactory connFactory;

        public void Add(ITcpClient tcpClient)
        {
            var inboundSpooler = InboundMessageSpooler.Create(tcpClient);
            var outboundSpooler = OutboundMessageSpooler.Create(tcpClient);
            var connection = connFactory.Create(inboundSpooler, outboundSpooler, tcpClient);

            dict[(IPEndPoint) tcpClient.Client.RemoteEndPoint] = connection;

            connection.StartSpoolers();
        }

        public ConnectionManager(Dictionary<IPEndPoint, IPeerConnection> dict, IPeerConnectionFactory factory)
        {
            this.dict = dict;
            this.connFactory = factory;
        }

        public void CloseAllConnetions()
        {
            foreach(KeyValuePair<IPEndPoint, IPeerConnection> entry in dict)
            {
                entry.Value.Close();
            }
            dict.Clear();
        }
    }
}
