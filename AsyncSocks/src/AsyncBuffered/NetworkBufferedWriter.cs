using System;
using AsyncSocks.AsyncBuffered;

namespace AsyncSocks
{
    public class NetworkBufferedWriter : INetworkBufferedWriter
    {
        private ITcpClient tcpClient;

        public NetworkBufferedWriter(ITcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        public ITcpClient Client
        {
            get
            {
                return tcpClient;
            }
        }

        public void Write(byte[] message)
        {
            tcpClient.Write(message, 0, message.Length);
        }
    }
}