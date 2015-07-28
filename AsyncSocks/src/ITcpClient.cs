using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public interface ITcpClient
    {
        int Read(byte[] buffer, int offset, int lenght);
        void Write(byte[] buffer, int offset, int lenght);
        ISocket Client { get; }
        void Close();
        void Connect();
        void Connect(IPEndPoint remoteEndPoint);
    }
}
