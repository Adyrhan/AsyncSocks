using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Text;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class PeerConnectionTest
    {
        [TestMethod]
        public void SendMessageShouldTellOutboundMessageSpoolerToEnqueueTheMessage()
        {
            var inboundSpoolerMock = new Mock<IInboundMessageSpooler>();
            var outboundSpoolerMock = new Mock<IOutboundMessageSpooler>();

            var inboundSpooler = inboundSpoolerMock.Object;
            var outboundSpooler = outboundSpoolerMock.Object;

            PeerConnection connection = new PeerConnection(inboundSpooler, outboundSpooler);

            byte[] messageBytes = Encoding.ASCII.GetBytes("This is a test message");

            outboundSpoolerMock.Setup(x => x.Enqueue(messageBytes)).Verifiable();
            
            connection.SendMessage(messageBytes);

            outboundSpoolerMock.Verify();
        }
    }
}
