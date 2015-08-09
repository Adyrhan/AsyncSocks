using System;

namespace AsyncSocks
{
    public delegate void NewMessageReceived(object sender, NewMessageReceivedEventArgs e);

    public class NewMessageReceivedEventArgs : EventArgs
    {
        //private IAsyncClient asyncClient;
        //private byte[] message;

        public NewMessageReceivedEventArgs(IAsyncClient sender, byte[] message)
        {
            Sender = sender;
            Message = message;
        }

        public IAsyncClient Sender { get; }
        public byte[] Message { get; }
    }
}