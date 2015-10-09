using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class NetworkStreamReader : INetworkStreamReader
    {
        private int bufferSize;
        public NetworkStreamReader(ITcpClient tcpClient, int bufferSize)
        {
            this.bufferSize = bufferSize;
            Client = tcpClient;
        }

        public ITcpClient Client { get; }

        public ReadResult<byte[]> Read()
        {
            try
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead = Client.Read(buffer, 0, bufferSize);

                if (bytesRead == 0)
                {
                    return null; // Peer disconnected
                }
                else
                {
                    return new ReadResult<byte[]>(buffer);
                }
            }
            catch (IOException)
            {
                return null; // Closing connection from another thread
            }
        }
    }
}
