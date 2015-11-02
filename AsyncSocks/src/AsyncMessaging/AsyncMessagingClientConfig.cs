using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks.AsyncMessaging
{
    /// <summary>
    /// Subclass of ClientConfig object used by AsyncMessagingClientFactory. The static GetDefault() method returns an instance of this configuration object that only allows messages to be sent and received of up to 8MB of size.
    /// </summary>
    public class AsyncMessagingClientConfig : ClientConfig
    {
        public AsyncMessagingClientConfig(Dictionary<string, string> keyValuePairs) : base(keyValuePairs) { }

        /// <summary>
        /// Returns an instance of this configuration object that only allows messages to be sent and received of up to 8MB of size.
        /// </summary>
        /// <returns>Instance of AsyncMessagingClientConfig</returns>
        public static AsyncMessagingClientConfig GetDefault()
        {
            var kvp = new Dictionary<string, string>();
            kvp.Add("MaxMessageSize", (8 * 1024 * 1024).ToString());
            return new AsyncMessagingClientConfig(kvp);
        }
    }
}
