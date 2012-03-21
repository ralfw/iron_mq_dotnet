using System;

namespace io.iron.ironmq.Data
{
    [Serializable]
    public class QueueMessages
    {
        public Message[] messages{ get; set; }       
    }
}
