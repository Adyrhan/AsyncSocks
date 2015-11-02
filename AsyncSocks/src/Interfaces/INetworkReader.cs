using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Interface for the implementation of a NetworkReader.
    /// The implementations of this interface have the responsibility to read and parse the data coming from 
    /// a remote end into an understandable message following a protocol.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public interface INetworkReader<T>
    {
        /// <summary>
        /// The implementation of this method should read and format the data into a message given a protocol.
        /// If this method returns null, it means the client has disconnected.
        /// </summary>
        /// <returns>A ReadResult instance if a message has been read or an error has ocurred while reading. Null if client has disconnected.</returns>
        ReadResult<T> Read();
        /// <summary>
        /// Instance of a wrapped TcpClient object used by this reader.
        /// </summary>
        ITcpClient Client { get; }
    }
}
