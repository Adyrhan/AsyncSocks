using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    class BaseTcpListener : ITcpListener
    {
        private TcpListener listener;

        public BaseTcpListener(TcpListener listener)
        {
            this.listener = listener;
        }

        public ITcpClient AcceptTcpClient()
        {
            return new BaseTcpClient(listener.AcceptTcpClient());
        }

        public void Stop()
        {
            listener.Stop();
        }

        public void Start()
        {
            listener.Start();
        }
    }
}
