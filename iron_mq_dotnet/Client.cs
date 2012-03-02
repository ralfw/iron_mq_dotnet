using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

using io.iron.ironmq.Data;

namespace io.iron.ironmq
{
    public class Client
    {
        private const string        PROTO =         "https";
        private const string        HOST =          "mq-aws-us-east-1.iron.io";
        private const int           PORT =          443;
        private const string        API_VERSION =   "1";


        private string projectId = string.Empty;
        private string token = string.Empty;

        public string Host { get; private set; }
        public int Port { get; private set; }

        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// Constructs a new Client using the specified project ID and token.
        ///  The network is not accessed during construction and this call will
        /// succeed even if the credentials are invalid.
        /// </summary>
        /// <param name="projectId">projectId A 24-character project ID.</param>
        /// <param name="token">token An OAuth token.</param>
        public Client(string projectId, string token, string host = HOST, int port = PORT)
        {
            this.projectId = projectId;
            this.token = token;
            this.Host = host;
            this.Port = port;
        }


       
        /// <summary>
        /// Returns a Queue using the given name.
        /// The network is not accessed during this call.
        /// </summary>
        /// <param name="name">param name The name of the Queue to create.</param>
        /// <returns></returns>
        public Queue queue (string name)
		{
			return new Queue (this, name);           
		}
		
		/// <summary>
		/// Returns list of queues
		/// </summary>
		/// <param name="page">
		/// Queue list page
		/// </param>
		public string[] queues (int page = 0)
		{
			string ep = "queues";
			if (page != 0) {
				ep += "?page=" + page.ToString ();
			}
			return serializer.Deserialize<string[]> (@get (ep));
		}

        public string delete(string endpoint)
        {
            return request("DELETE", endpoint, null);
        }

        public string get(string endpoint)
        {
            return request("GET", endpoint, null);
        }

        public string post(string endpoint, string body)
        {
            return request("POST", endpoint, body);
        }

        private string request(string method, string endpoint, string body)
        {
            string path = "/" + API_VERSION + "/projects/" + projectId + "/" + endpoint;
            string uri = PROTO + "://" + this.Host + ":" + this.Port + path;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "OAuth " + token);
            request.UserAgent = "IronMQ .Net Client";
            request.Method = method;
            if (body != null)
            {                
                using (System.IO.StreamWriter write = new System.IO.StreamWriter(request.GetRequestStream()))
                {
                    write.Write(body);
                    write.Flush();
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string json = string.Empty;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                json = reader.ReadToEnd();
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Error error = serializer.Deserialize<Error>(json);
                throw new System.Web.HttpException((int)response.StatusCode, error.msg);
            }
            return json;            
        }

    }
}
