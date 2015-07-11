using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class PeerConnection
    {
        private IInboundMessageSpooler inboundSpooler;
        private IOutboundMessageSpooler outboundSpooler;
        private ITcpClient tcpClient;

        public PeerConnection(IInboundMessageSpooler inboundSpooler, IOutboundMessageSpooler outboundSpooler, ITcpClient tcpClient)
        {
            this.inboundSpooler = inboundSpooler;
            this.outboundSpooler = outboundSpooler;
            this.tcpClient = tcpClient;
        }

        public void SendMessage(byte[] messageBytes)
        {
            outboundSpooler.Enqueue(messageBytes);
        }

        public void StartSpoolers()
        {
            inboundSpooler.Start();
            outboundSpooler.Start();
        }

        public void Close()
        {
            inboundSpooler.Stop();
            outboundSpooler.Stop();

            tcpClient.Close();
        }
    }
}
