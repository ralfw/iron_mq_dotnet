using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace io.iron.ironmq.Data
{
    [Serializable]
    public class QueueMessages
    {
        public Message[] messages{ get; set; }       
    }
}
