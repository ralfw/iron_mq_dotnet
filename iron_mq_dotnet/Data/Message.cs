using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace io.iron.ironmq.Data
{
    [Serializable]
    public class Message
    {        
        public string Id { get; set; }
        public string Body { get; set; }
        public long Timeout { get; set; }
    }
}
