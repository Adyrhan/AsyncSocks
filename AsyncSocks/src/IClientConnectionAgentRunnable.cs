using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IClientConnectionAgentRunnable : IRunnable
    {
        void AcceptClientConnection();
        event NewPeerConnectionDelegate OnNewClientConnection;
    }
}
