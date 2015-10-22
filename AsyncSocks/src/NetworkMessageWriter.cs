using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Implementation of INetworkWriter that writes binary messages to the network, for the connected client to read.
    /// </summary>
    public class NetworkMessageWriter : INetworkMessageWriter
    {
        public NetworkMessageWriter(ITcpClient client)
        {
            Client = client;
        }

        /// <summary>
        /// Wrapped instance of TcpClient for the client connection used to send messages.
        /// </summary>
        public ITcpClient Client { get; }

        /// <summary>
        /// Writes the given array of bytes to the network. Checks the size of the array and sends 4bytes with the size of that array. Then sends the contents of the array.
        /// </summary>
        /// <param name="message">Binary message to send.</param>
        public void Write(byte[] message)
        {
            byte[] size = BitConverter.GetBytes(message.Length);
            int totalSize = size.Length + message.Length;

            byte[] packetBytes = new byte[totalSize];

            for (int i = 0; i < totalSize; i++)
            {
                if (i < size.Length)
                {
                    packetBytes[i] = size[i];
                }
                else
                {
                    packetBytes[i] = message[i - size.Length];
                }
            }

            Client.Write(packetBytes, 0, totalSize);
        }
    }
}
