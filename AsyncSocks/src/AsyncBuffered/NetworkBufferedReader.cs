using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks.AsyncBuffered
{
    /// <summary>
    /// Implementation of INetworkReader for buffered reads from a client.
    /// </summary>
    public class NetworkBufferedReader : INetworkBufferedReader
    {
        private int bufferSize;
        public NetworkBufferedReader(ITcpClient tcpClient, int bufferSize)
        {
            this.bufferSize = bufferSize;
            Client = tcpClient;
        }

        public ITcpClient Client { get; }

        /// <summary>
        /// Reads up to the buffer size specified when creating the instance of this object, and returns the read data.
        /// </summary>
        /// <returns>Read data coming from the client. Has a maximum size equal to the buffer size specified when creating the instance of this object.</returns>
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
