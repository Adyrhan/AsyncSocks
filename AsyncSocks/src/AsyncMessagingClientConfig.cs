using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class AsyncMessagingClientConfig : ClientConfig
    {
        public AsyncMessagingClientConfig(Dictionary<string, string> keyValuePairs) : base(keyValuePairs) { }
        public static AsyncMessagingClientConfig GetDefault()
        {
            var kvp = new Dictionary<string, string>();
            kvp.Add("MaxMessageSize", (8 * 1024 * 1024).ToString());
            return new AsyncMessagingClientConfig(kvp);
        }
    }
}
