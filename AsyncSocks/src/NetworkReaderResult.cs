using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class NetworkReaderResult<T>
    {
        public Exception Error { get; }
        public T Result { get; }

        public NetworkReaderResult(T result, Exception error)
        {
            Error = error;
            Result = result;
        }

        public NetworkReaderResult(T result) : this(result, null) { }
        public NetworkReaderResult(Exception error) : this(default(T), error) { }
    }
}
