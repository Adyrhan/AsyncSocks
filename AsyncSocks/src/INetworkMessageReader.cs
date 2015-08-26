using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface INetworkMessageReader
    {
        byte[] Read();
        ITcpClient Client { get; }
    }
}
