using AsyncSocks;
using AsyncSocks.AsyncBuffered;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class NetworkBufferedWriterTests
    {
        private INetworkBufferedWriter writer;
        private Mock<ITcpClient> tcpClientMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpClientMock = new Mock<ITcpClient>();
            writer = new NetworkBufferedWriter(tcpClientMock.Object);
        }

        [TestMethod]
        public void WritesAllBytesToNetwork()
        {
            byte[] message = new byte[2048];

            Func<byte[], byte[], bool> compareArrays = (array1, array2) =>
            {
                if (array1.Length != array2.Length) return false;

                for (int i = 0; i < array1.Length; i++)
                {
                    if (array1[i] != array2[i]) return false;
                }

                return true;
            };

            tcpClientMock.Setup(x => x.Write(It.Is<byte[]>(ba => compareArrays(message, ba)), 0, 2048)).Verifiable();
            writer.Write(message);
            tcpClientMock.Verify();
        }
    }
}
