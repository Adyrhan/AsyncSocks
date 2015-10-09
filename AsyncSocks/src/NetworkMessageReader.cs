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
        private int maxMessageSize;

        public NetworkMessageReader(ITcpClient tcpClient, int maxMessageSize)
        {
            this.tcpClient = tcpClient;
            this.maxMessageSize = maxMessageSize;
        }

        public ITcpClient Client
        {
            get { return tcpClient; }
        }

        public ReadResult<byte[]> Read()
        {
            try
            {
                byte[] buffer = new byte[4];
                if (tcpClient.Read(buffer, 0, 4) == 0)
                {
                    return null;
                }
                int messageLength = BitConverter.ToInt32(buffer, 0);

                if (messageLength > maxMessageSize)
                {
                    return null;
                }

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
                return new ReadResult<byte[]>(buffer);
            }
            catch (IOException)
            {
                return null;
            }
            
        }
    }
}
