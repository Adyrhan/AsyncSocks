using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AsyncSocks
{
    public interface ISocket
    {
        EndPoint RemoteEndPoint { get; }
        EndPoint LocalEndPoint { get; }
    }
}
