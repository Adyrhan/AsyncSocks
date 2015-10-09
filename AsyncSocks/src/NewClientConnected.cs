using System;

namespace AsyncSocks
{
    public delegate void NewClientConnected<T>(object sender, NewClientConnectedEventArgs<T> e);

    public class NewClientConnectedEventArgs<T> : EventArgs
    {
        public NewClientConnectedEventArgs(IAsyncClient<T> client)
        {
            Client = client;
        }

        public IAsyncClient<T> Client { get; }
    }
}