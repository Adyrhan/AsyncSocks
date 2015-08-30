using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public class NetworkMessageReader : INetworkMessageReader
    {
        private ITcpClient tcpClient;

        public NetworkMessageReader(ITcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        public ITcpClient Client
        {
            get { return tcpClient; }
        }

        public byte[] Read()
        {
            try
            {
                byte[] buffer = new byte[4];
                if (tcpClient.Read(buffer, 0, 4) == 0)
                {
                    return null;
                }
                int messageLength = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[messageLength];
                int bytesRead = 0;
                while (bytesRead < messageLength)
                {
                    var bytesReceived = tcpClient.Read(buffer, bytesRead, messageLength - bytesRead);
                    if (bytesReceived == 0)
                    {
                        return null;
                    }
                    bytesRead += bytesReceived;
                }
                return buffer;
            }
            catch (IOException)
            {
                return null;
            }
            
        }
    }
}
