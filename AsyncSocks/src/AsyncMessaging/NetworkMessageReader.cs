using AsyncSocks.AsyncMessaging.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks.AsyncMessaging
{
    /// <summary>
    /// Implementation of INetworkReader for the use of AsyncMessagingClient.
    /// </summary>
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

        /// <summary>
        /// When called, reads the first 4 bytes coming from the client and uses them as the size for the message that will be 
        /// read inmediatelly before those 4 bytes.
        /// </summary>
        /// <returns>Binary message in the form of an array of bytes.</returns>
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

                if (messageLength > maxMessageSize || messageLength < 1)
                {
                    return new ReadResult<byte[]>(new MessageTooBigException("Received message of size "+messageLength.ToString()+" not in range 0 to "+maxMessageSize));
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
