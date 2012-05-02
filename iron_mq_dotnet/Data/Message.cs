using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace io.iron.ironmq.Data
{
    [Serializable]
    [JsonObject]
    public class Message
    {
        public string Id { get; set; }

        public string Body { get; set; }

        [DefaultValue(0)]
        public long Timeout { get; set; }

        [DefaultValue(0)]
        public long Delay { get; set; }

        [DefaultValue(0)] 
        public long Expires_In { get; set; }
    }
}
