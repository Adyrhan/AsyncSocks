﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public delegate void NewClientConnectionDelegate(ITcpClient client);
    public class ClientConnectionAgentRunnable : IClientConnectionAgentRunnable
    {
        private ITcpListener listener;        
        private bool running;
        private bool shouldStop;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private IAsyncClientFactory connectionFactory;

        public event NewPeerConnectionDelegate OnNewClientConnection;

        public bool IsRunning
        {
            get { return running; }
        }

        public ITcpListener TcpListener
        {
            get
            {
                return listener;
            }
        }

        public void Stop()
        {
            shouldStop = true;
            listener.Stop();
        }

        public bool WaitStarted()
        {
            return startedEvent.WaitOne();
        }
        
        public ClientConnectionAgentRunnable(ITcpListener listener, IAsyncClientFactory connectionFactory)
        {
            this.listener = listener;
            this.connectionFactory = connectionFactory;
        }

        public void AcceptClientConnection()
        {
            try
            {
                ITcpClient client = listener.AcceptTcpClient();
                if (OnNewClientConnection != null)
                {
                    OnNewClientConnection(connectionFactory.Create(client));
                }
            }
            catch (SocketException)
            {
                shouldStop = true;
            }
        }

        public void Run()
        {
            running = true;
            startedEvent.Set();
            listener.Start();
            while (!shouldStop)
            {
                AcceptClientConnection();
            }
            
            running = false;
        }
    }
}
