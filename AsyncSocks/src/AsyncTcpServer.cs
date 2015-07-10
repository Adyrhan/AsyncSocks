using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocks
{
    public class AsyncTcpServer
    {
        private TcpListener listener;
        private MessageParser parser;
        private MessageReceivedCallback messageReceivedCallback;
        public static AsyncTcpServer Create(IPEndPoint listeningEndPoint, MessageReceivedCallback messageReceivedCallback)
        {
            if (listeningEndPoint == null) throw new ArgumentNullException("listeningEndPoint cannot be null");
            if (messageReceivedCallback == null) throw new ArgumentNullException("messageReceivedCallback cannot be null");
            
            return new AsyncTcpServer(listeningEndPoint, messageReceivedCallback);

        }

        public AsyncTcpServer(IPEndPoint listeningEndPoint,  MessageReceivedCallback callback)
        {
            this.messageReceivedCallback = callback;
        }


    }
}
