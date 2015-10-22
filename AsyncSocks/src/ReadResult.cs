using System;

namespace AsyncSocks
{
    /// <summary>
    /// This is the result object of INetworkReader.Read(). See <see cref="INetworkReader{T}.Read"/>.
    /// It can contain either a protocol message or an Exception in case of error.
    /// </summary>
    /// <typeparam name="T">Type for the message associated with the protocol that the instance of AsyncClient is using.</typeparam>
    public class ReadResult<T>
    {
        /// <summary>
        /// The error that happened when reading the message.
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// The message contents.
        /// </summary>
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