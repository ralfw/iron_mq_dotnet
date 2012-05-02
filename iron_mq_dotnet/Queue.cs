using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using io.iron.ironmq.Data;
using Newtonsoft.Json;

namespace io.iron.ironmq
{
    /// <summary>
    /// Represends a specific IronMQ Queue.
    /// </summary>
    public class Queue
    {
        private Client client = null;
        private string name = null;
        private JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.None, DefaultValueHandling = DefaultValueHandling.Ignore };

        public Queue(Client client, string name)
        {
            this.client = client;
            this.name = name;
        }

        /// <summary>
        /// Clears a Queue regardless of message status
        /// </summary>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void clear()
        {
            string emptyJsonObject = "{}";
            var response = client.post("queues/" + name + "/clear", emptyJsonObject);
            var responseObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(response,settings);
            if (responseObject["msg"] != "Cleared")
            {
                throw new Exception(string.Format("Unknown response from REST Endpoint : {0}", response));
            }
        }


        /// <summary>
        /// Retrieves a Message from the queue. If there are no items on the queue, an HTTPException is thrown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public Message get()
        {
            string json = client.get("queues/" + name + "/messages");
            QueueMessages queueResp = JsonConvert.DeserializeObject<QueueMessages>(json, settings);
            return queueResp.messages.Length > 0 ? queueResp.messages[0] : null;
        }

        /// <summary>
        /// Retrieves up to "max" messages from the queue
        /// </summary>
        /// <param name="max">the count of messages to return, default is 1</param>
        /// <returns>An IList of messages</returns>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public IList<Message> get(int max = 1)
        {
            string json = client.get(string.Format("queues/{0}/messages?n={1}", name, max));
            QueueMessages queueResp = JsonConvert.DeserializeObject<QueueMessages>(json,settings);
            return queueResp.messages;
        }

        /// <summary>
        /// Delete a message from the queue
        /// </summary>
        /// <param name="id">Message Identifier</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void deleteMessage(String id)
        {
            client.delete("queues/" + name + "/messages/" + id);
        }



        /// <summary>
        /// Delete a message from the queue
        /// </summary>
        /// <param name="msg">Message to be deleted</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void deleteMessage(Message msg)
        {
            deleteMessage(msg.Id);
        }



        /// <summary>
        /// pushes a message onto the queue
        /// </summary>
        /// <param name="msg">Message to be pushed</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void push(String msg)
        {
            push(msg, 0);
        }

   
        /// <summary>
        /// Pushes a message onto the queue with a timeout
        /// </summary>
        /// <param name="msg">Message to be pushed.</param>
        /// <param name="timeout">The timeout of the message to push.</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void push(String msg, long timeout)
        {
            push(new string[] { msg }, timeout);
        }

        /// <summary>
        /// Pushes messages onto the queue with an optional timeout
        /// </summary>
        /// <param name="msg">Messages to be pushed.</param>
        /// <param name="timeout">The timeout of the messages to push.</param>
        /// <exception cref="System.Web.HttpException">Thown if the IronMQ service returns a status other than 200 OK. </exception>
        /// <exception cref="System.IO.IOException">Thrown if there is an error accessing the IronMQ server.</exception>
        public void push(IEnumerable<string> msgs, long timeout = 0, long delay = 0, long expires_in = 0)
        {
            var json =  JsonConvert.SerializeObject(new QueueMessages()
                {
                    messages = msgs.Select(msg => new Message() { Body = msg, Timeout = timeout, Delay = delay, Expires_In = expires_in }).ToArray(),
                }, settings);
            client.post("queues/" + name + "/messages", json
               );
        }
    }
}