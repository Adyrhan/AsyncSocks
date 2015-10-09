using System;

namespace AsyncSocks
{
    public class ReadResult<T>
    {
        public Exception Error { get; }
        public T Message { get; }

        public ReadResult(T message, Exception error)
        {
            Error = error;
            Message = message;
        }

        public ReadResult(T message) : this(message, null) { }
        public ReadResult(Exception error) : this(default(T), error) { }
    }
}