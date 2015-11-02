using System;

namespace AsyncSocks
{
    public class ReadErrorEventArgs
    {
        public Exception Error { get; }

        public ReadErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}