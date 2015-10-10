using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class NetworkMessageWriter : INetworkMessageWriter
    {
        public NetworkMessageWriter(ITcpClient client)
        {
            Client = client;
        }

        public ITcpClient Client { get; }

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
