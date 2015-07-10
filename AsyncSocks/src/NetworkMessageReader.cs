using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public class NetworkMessageReader : INetworkMessageReader
    {
        private ITcpClient tcpClient;

        public NetworkMessageReader(ITcpClient tcpClient)
        {
            // TODO: Complete member initialization
            this.tcpClient = tcpClient;
        }

        public byte[] Read()
        {
            byte[] buffer = new byte[4];
            tcpClient.Read(buffer, 0, 4);
            int messageLenght = BitConverter.ToInt32(buffer, 0);
            
            buffer = new byte[messageLenght];
            int bytesRead = 0;
            while (bytesRead < messageLenght)
            {
                bytesRead += tcpClient.Read(buffer, bytesRead, messageLenght - bytesRead);
            }
            return buffer;
        }
    }
}
