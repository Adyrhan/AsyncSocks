using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace AsyncSocks
{
    public class BaseTcpClient : ITcpClient
    {
        private TcpClient tcpClient;
        private IPEndPoint remoteEndPoint;

        public BaseTcpClient(TcpClient tcpClient) : this(tcpClient, null) {}

        public BaseTcpClient(TcpClient tcpClient, IPEndPoint remoteEndPoint)
        {
            this.tcpClient = tcpClient;
            this.remoteEndPoint = remoteEndPoint;
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return remoteEndPoint; }
            set { remoteEndPoint = value; }
        }

        public int Read(byte[] buffer, int offset, int lenght)
        {
            return tcpClient.GetStream().Read(buffer, offset, lenght);
        }

        public void Write(byte[] buffer, int offset, int lenght)
        {
            tcpClient.GetStream().Write(buffer, offset, lenght);
        }

        public void Close()
        {
            tcpClient.Close();
        }

        public void Connect()
        {
            Connect(remoteEndPoint);
        }

        public void Connect(IPEndPoint remoteEndPoint)
        {
            tcpClient.Connect(remoteEndPoint);
        }

        public ISocket Client
        {
            get { return new BaseSocket(tcpClient.Client); }
        }
    }
}
