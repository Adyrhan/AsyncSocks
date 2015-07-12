using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    class BaseSocket : ISocket
    {
        private Socket socket;
        
        public BaseSocket(Socket socket)
        {
            this.socket = socket;
        }

        public EndPoint RemoteEndPoint
        {
            get { return socket.RemoteEndPoint; }
        }
    }
}
