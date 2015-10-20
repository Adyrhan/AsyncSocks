using System.Collections.Generic;

namespace AsyncSocks
{
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

        public string GetProperty(string key)
        {
            return dict[key];
        }
    }
}