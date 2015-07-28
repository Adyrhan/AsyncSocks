using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class NetworkMessage
    {
        private IAsyncClient sender;
        private byte[] message;

        public NetworkMessage(IAsyncClient sender, byte[] message)
        {
            this.sender = sender;
            this.message = message;
        }

        public IAsyncClient Sender { get { return sender;  } }
        public byte[] Message { get { return message;  } }
    }
}
