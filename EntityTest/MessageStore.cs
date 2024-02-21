using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityTest
{
    public class Message
    {
            public Int32 MsgID { get; set; }
            public String? Text { get; set; }
            public DateTime DateReceived { get; set; }
            public MessageCollection? MessageCollection { get; set; } //a message must belong to a messagecollection
      
    }
    public class MessageCollection
    {
        public virtual ICollection<Message>? Messages { get; set; }
        public String? UserID { get; set; }
    }
}
