using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Wrapped instance of TcpListener
    /// </summary>
    public class BaseTcpListener : ITcpListener
    {
        private TcpListener listener;

        public BaseTcpListener(TcpListener listener)
        {
            this.listener = listener;
        }

        /// <summary>
        /// Accepts a single incomming client connection.
        /// </summary>
        /// <returns>Wrapped instance of TcpClient</returns>
        public ITcpClient AcceptTcpClient()
        {
            
            TcpClient client = listener.AcceptTcpClient();
            return new BaseTcpClient(client);
        }

        /// <summary>
        /// Stops the TcpListener from listening.
        /// </summary>
        public void Stop()
        {
            listener.Stop();
        }

        /// <summary>
        /// Starts TcpListener for listening new client connections.
        /// </summary>
        public void Start()
        {
            listener.Start();
        }
    }
}
