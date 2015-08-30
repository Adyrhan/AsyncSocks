using System.Net;

namespace AsyncSocks
{
    public interface ITcpClient
    {
        bool Connected { get; }
        ISocket Socket { get; }
        int Read(byte[] buffer, int offset, int lenght);
        void Write(byte[] buffer, int offset, int lenght);
        void Close();
        void Connect();
        void Connect(IPEndPoint remoteEndPoint);
    }
}
