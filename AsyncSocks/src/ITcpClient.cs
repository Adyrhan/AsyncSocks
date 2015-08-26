using System.Net;

namespace AsyncSocks
{
    public interface ITcpClient
    {
        int Read(byte[] buffer, int offset, int lenght);
        void Write(byte[] buffer, int offset, int lenght);
        ISocket Socket { get; }
        void Close();
        void Connect();
        void Connect(IPEndPoint remoteEndPoint);
    }
}
