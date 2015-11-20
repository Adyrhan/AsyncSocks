namespace AsyncSocks
{
    public class AsyncBufferedClient : AsyncClient<byte[]>
    {
        public AsyncBufferedClient
        (
            IInboundMessageSpooler<byte[]> inboundSpooler, 
            IOutboundMessageSpooler<byte[]> outboundSpooler, 
            IMessagePoller<byte[]> poller, 
            IOutboundMessageFactory<byte[]> messageFactory,
            ITcpClient tcpClient,
            AsyncBufferedClientConfig config
        ) : base (inboundSpooler, outboundSpooler, poller, messageFactory, tcpClient, config) { }

        protected override void RaiseOnReadError(object sender, ReadErrorEventArgs e)
        {
            base.RaiseOnReadError(sender, e);
            Disconnect();
        }
    }
}