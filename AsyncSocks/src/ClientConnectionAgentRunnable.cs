using System;
using System.Collections.Generic;
using System.Linq;
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
        private IPeerConnectionFactory connectionFactory;

        public event NewPeerConnectionDelegate OnNewClientConnection;
        

        public bool IsRunning
        {
            get { return running; }
        }

        public void Stop()
        {
            shouldStop = true;
        }

        public bool WaitStarted()
        {
            return startedEvent.WaitOne();
        }
        
        public ClientConnectionAgentRunnable(ITcpListener listener, IPeerConnectionFactory connectionFactory)
        {
            this.listener = listener;
            this.connectionFactory = connectionFactory;
        }

        public void AcceptClientConnection()
        {
            ITcpClient client = listener.AcceptTcpClient();
            if (OnNewClientConnection != null)
            {
                OnNewClientConnection(connectionFactory.Create(client));
            }
        }

        public void Run()
        {
            running = true;
            startedEvent.Set();
            while (!shouldStop)
            {
                AcceptClientConnection();
            }
            running = false;
        }
    }
}
