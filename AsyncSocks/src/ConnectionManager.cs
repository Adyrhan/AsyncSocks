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
        private IPeerConnectionFactory connFactory;
        private IMessagePoller poller;

        public event NewClientMessageDelegate OnNewClientMessageReceived;

        public void Add(ITcpClient tcpClient)
        {
            var inboundSpooler = InboundMessageSpooler.Create(tcpClient);
            var outboundSpooler = OutboundMessageSpooler.Create(tcpClient);
            var messagePoller = MessagePoller.Create(inboundSpooler.Queue);
            Add(inboundSpooler, outboundSpooler, poller, tcpClient);
        }

        public ConnectionManager(Dictionary<IPEndPoint, IPeerConnection> dict, IPeerConnectionFactory factory, IMessagePoller poller)
        {
            this.dict = dict;
            this.connFactory = factory;
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


        public void Add(IInboundMessageSpooler inbound, IOutboundMessageSpooler outbound, IMessagePoller poller, ITcpClient tcpClient)
        {
            var connection = connFactory.Create(inbound, outbound, poller, tcpClient);
            dict[(IPEndPoint)tcpClient.Client.RemoteEndPoint] = connection;

            connection.Start();
        }
    }
}
