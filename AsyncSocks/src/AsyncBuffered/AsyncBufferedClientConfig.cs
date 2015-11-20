using System.Collections.Generic;

namespace AsyncSocks
{
    public class AsyncBufferedClientConfig : ClientConfig
    {
        public AsyncBufferedClientConfig(Dictionary<string, string> dict) : base(dict) { }

        public static AsyncBufferedClientConfig GetDefault()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("BufferSize", (1024 * 12).ToString());
            return new AsyncBufferedClientConfig(dict);
        }
    }
}