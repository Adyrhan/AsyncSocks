using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public delegate void NewClientConnectionDelegate(ITcpClient client);
    public class ClientConnectionAgent : IRunnable
    {
        private ITcpListener listener;        
        private bool running;
        private bool shouldStop;
        private AutoResetEvent startedEvent = new AutoResetEvent(false);

        public event NewClientConnectionDelegate OnNewClientConnection;
        

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
            return startedEvent.WaitOne(2000);
        }
        
        public ClientConnectionAgent(ITcpListener listener)
        {
            this.listener = listener;
        }

        public void AcceptClientConnection()
        {
            ITcpClient client = listener.AcceptTcpClient();
            OnNewClientConnection(client);
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
