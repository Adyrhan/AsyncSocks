using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace AsyncSocks
{
    public class BaseTcpClient : ITcpClient, IDisposable
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

        public int Read(byte[] buffer, int offset, int length)
        {
            return tcpClient.GetStream().Read(buffer, offset, length);
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            tcpClient.GetStream().Write(buffer, offset, length);
        }

        public void Close()
        {
            Dispose();
        }

        public void Connect()
        {
            Connect(remoteEndPoint);
        }

        public void Connect(IPEndPoint remoteEndPoint)
        {
            tcpClient.Connect(remoteEndPoint);
        }

        public ISocket Socket
        {
            get { return new BaseSocket(tcpClient.Client); }
        }

        public bool Connected
        {
            get
            {
                return tcpClient.Connected;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Disposing of managed state goes here (managed objects).
                    tcpClient.GetStream().Dispose();
                    tcpClient.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

        }
        #endregion
    }
}
