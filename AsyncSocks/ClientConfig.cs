namespace AsyncSocks
{
    public class ClientConfig
    {
        public int MaxMessageSize { get; }

        public ClientConfig(int maxMessageSize)
        {
            MaxMessageSize = maxMessageSize;
        }

        public override string ToString()
        {
            return "MaxMessageSize: " + MaxMessageSize.ToString();
        }

        public static ClientConfig GetDefault()
        {
            return new ClientConfig(10 * 1024 * 1024); // 10MB max message size
        }
    }
}