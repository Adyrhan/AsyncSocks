using System.Collections.Generic;

namespace AsyncSocks
{
    /// <summary>
    /// Configuration object used by <see cref="AsyncClientFactory{T}"/> to create instances of <see cref="AsyncClient{T}"/>.
    /// </summary>
    public class ClientConfig
    {
        private Dictionary<string, string> dict;

        public ClientConfig(Dictionary<string, string> keyValuePairs)
        {
            dict = keyValuePairs;
        }

        public override string ToString()
        {
            return dict.ToString();
        }

        /// <summary>
        /// Gets the value for the given key.
        /// </summary>
        /// <param name="key">Key for the value.</param>
        /// <returns>Value for the given key.</returns>
        public string GetProperty(string key)
        {
            return dict[key];
        }
    }
}