﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    /// <summary>
    /// Delegate for NewClientConnection events
    /// </summary>
    /// <param name="client">The wrapped TcpClient instance associated to the new client.</param>
    public delegate void NewClientConnectionDelegate(ITcpClient client);

    /// <summary>
    /// This is the runnable that holds the code executed by the internal thread of ClientConnectionAgent. See <see cref="ClientConnectionAgent{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type for the message object associated with the protocol that the AsyncClient instance is using.</typeparam>
    public class ClientConnectionAgentRunnable<T> : IClientConnectionAgentRunnable<T>, IDisposable
    {
        private ITcpListener listener;        
        private bool running;
        private bool shouldStop;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);
        private IAsyncClientFactory<T> connectionFactory;

        public event NewClientConnected<T> OnNewClientConnection;

        public bool IsRunning
        {
            get { return running; }
        }

        public ITcpListener TcpListener
        {
            get { return listener; }
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
        
        public ClientConnectionAgentRunnable(ITcpListener listener, IAsyncClientFactory<T> connectionFactory)
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
                    var e = new NewClientConnectedEventArgs<T>(connectionFactory.Create(client));
                    OnNewClientConnection(this, e);
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

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    startedEvent.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
