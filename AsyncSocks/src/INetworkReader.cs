using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public interface INetworkReader<T>
    {
        NetworkReaderResult<T> Read();
        ITcpClient Client { get; }
    }
}
