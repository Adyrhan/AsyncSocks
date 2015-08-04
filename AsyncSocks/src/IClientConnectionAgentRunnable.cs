using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IClientConnectionAgentRunnable : IRunnable
    {
        ITcpListener TcpListener { get; }

        void AcceptClientConnection();
        event NewPeerConnectionDelegate OnNewClientConnection;
    }
}
