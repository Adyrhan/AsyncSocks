using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace AsyncSocks
{
    public class BaseTcpClient : ITcpClient
    {
        private TcpClient tcpClient;

        public BaseTcpClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
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
    }
}
