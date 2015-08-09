using System;

namespace AsyncSocks
{
    public delegate void NewClientConnected(object sender, NewClientConnectedEventArgs e);

    public class NewClientConnectedEventArgs : EventArgs
    {
        public NewClientConnectedEventArgs(IAsyncClient client)
        {
            Client = client;
        }

        public IAsyncClient Client { get; }
    }
}