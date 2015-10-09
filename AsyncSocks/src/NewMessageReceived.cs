using System;

namespace AsyncSocks
{
    public delegate void NewMessageReceived<T>(object sender, NewMessageReceivedEventArgs<T> e);

    public class NewMessageReceivedEventArgs<T> : EventArgs
    {
        //private IAsyncClient asyncClient;
        //private byte[] message;

        public NewMessageReceivedEventArgs(IAsyncClient<T> sender, T message)
        {
            Sender = sender;
            Message = message;
        }

        public IAsyncClient<T> Sender { get; }
        public T Message { get; }
    }
}