using System;

namespace io.iron.ironmq.Data
{
    [Serializable]
    public class Message
    {        
        public string Id { get; set; }
        public string Body { get; set; }
        public long Timeout { get; set; }
        public long Delay { get; set; }
        public long Expires_In { get; set; }
    }
}
