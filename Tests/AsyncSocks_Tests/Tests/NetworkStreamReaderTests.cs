using AsyncSocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO;
using System.Net.Sockets;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class NetworkStreamReaderTests
    {
        private INetworkStreamReader reader;
        private Mock<ITcpClient> tcpClientMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpClientMock = new Mock<ITcpClient>();
            reader = new NetworkStreamReader(tcpClientMock.Object, 1024 * 8);
        }

        [TestMethod]
        public void ReadShouldReturnNetworkReaderResultOnSuccess()
        {
            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, 1024 * 8)).Callback((byte[] buffer, int offset, int length) =>
            {
                for (int i = 0; i < 256; i++)
                {
                    buffer[i] = (byte)i;
                }
            }).Returns(256).Verifiable();

            ReadResult<byte[]> result = reader.Read();

            Assert.IsNotNull(result, "reader.Read() returned null");
            Assert.IsNull(result.Error , "result.Error is not null");
            Assert.IsNotNull(result.Message, "result.Result is null");
        }

        [TestMethod]
        public void ReadShouldReturnNullOnPeerDisconnection()
        {
            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, 1024 * 8)).Returns(0).Verifiable();

            ReadResult<byte[]> result = reader.Read();

            Assert.IsNull(result, "reader.Read() returned object");
        }

        [TestMethod]
        public void ReadShouldReturnNullOnClosedSocket()
        {
            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, 1024 * 8)).Throws(new SocketException()).Verifiable();

            ReadResult<byte[]> result = reader.Read();

            Assert.IsNull(result, "reader.Read() returned object");
        }
    }
}
