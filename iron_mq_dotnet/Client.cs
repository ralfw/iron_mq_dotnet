using System.Net;
using System.Web.Script.Serialization;

using io.iron.ironmq.Data;

namespace io.iron.ironmq
{
    public class Client
    {
        private const string PROTO_DEFAULT = "https";
        private const int PORT_DEFAULT = 443;
        private const string HOST_DEFAULT = "mq-aws-us-east-1.iron.io";

        private readonly JavaScriptSerializer _serializer;
        private readonly RESTadapter _rest;


        public string Host { get; private set; }
        public int Port { get; private set; }


        /// <summary>
        /// Constructs a new Client using the specified project ID and token.
        ///  The network is not accessed during construction and this call will
        /// succeed even if the credentials are invalid.
        /// </summary>
        /// <param name="projectId">projectId A 24-character project ID.</param>
        /// <param name="token">token An OAuth token.</param>
        public Client(string projectId, string token, string proto = PROTO_DEFAULT, string host = HOST_DEFAULT, int port = PORT_DEFAULT) 
                : this(new Credentials(projectId, token), proto, host, port) {}
        public Client(Credentials credentials, string proto = PROTO_DEFAULT, string host = HOST_DEFAULT, int port = PORT_DEFAULT)
        {
            this.Host = host;
            this.Port = port;

            _serializer = new JavaScriptSerializer();
            _rest = new RESTadapter(credentials, proto, host, port, _serializer);
        }


        /// <summary>
        /// Returns a Queue using the given name.
        /// The network is not accessed during this call.
        /// </summary>
        /// <param name="name">param name The name of the Queue to create.</param>
        /// <returns></returns>
        public Queue Queue(string name)
        {
            return new Queue(_rest, name);
        }

        public Queue this[string queueName]
        {
            get { return Queue(queueName); }
        }


        /// <summary>
        /// Returns list of queues
        /// </summary>
        /// <param name="page">
        /// Queue list page
        /// </param>
        public string[] QueuesNames(int page = 0)
        {
            var ep = "queues";
            if (page != 0) { ep += "?page=" + page.ToString (); }
            return _serializer.Deserialize<string[]>(_rest.Get(ep));
        }
    }
}
