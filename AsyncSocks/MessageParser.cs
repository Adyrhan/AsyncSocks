using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public delegate void MessageReceivedCallback(Message message, TcpClient client);

    public class Message
    {
        private byte[] message;
        private TcpClient sender;
        public byte[] MessageBytes
        {
            get { return message; }
            set { message = value; }
        }

        public TcpClient Sender
        {
            get { return sender; }
            set { sender = value; }
        }
    }

    public class MessageParser
    {
        public object StartReading(TcpListener tcpListener)
        {
            throw new NotImplementedException();
        }
    }
}
