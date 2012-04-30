using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using io.iron.ironmq.Data;

namespace io.iron.ironmq
{
    public class Queue
    {
        private Client client = null;
        private string name = null;
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public Queue(Client client, string name)
        {
            this.client = client;
            this.name = name;
        }

        public void clear()
        {
            string emptyJsonObject = "{}";
            var response = client.post("queues/" + name + "/clear", emptyJsonObject);
            var responseObject = serializer.Deserialize<Dictionary<string, string>>(response);
            if (responseObject["msg"] != "Cleared")
            {
                throw new Exception(string.Format("Unknown response from REST Endpoint : {0}", response));
            }
        }

        /**
        * Retrieves a Message from the queue. If there are no items on the queue, an
        * HTTPException is thrown.
        *
        * @throws HTTPException If the IronMQ service returns a status other than 200 OK.
        * @throws IOException If there is an error accessing the IronMQ server.
        */

        public Message get()
        {
            string json = client.get("queues/" + name + "/messages");
            QueueMessages queueResp = serializer.Deserialize<QueueMessages>(json);
            return queueResp.messages.Length > 0 ? queueResp.messages[0] : null;
        }

        /**
        * Deletes a Message from the queue.
        *
        * @param id The ID of the message to delete.
        *
        * @throws HTTPException If the IronMQ service returns a status other than 200 OK.
        * @throws IOException If there is an error accessing the IronMQ server.
        */

        public void deleteMessage(String id)
        {
            client.delete("queues/" + name + "/messages/" + id);
        }

        /**
        * Deletes a Message from the queue.
        *
        * @param msg The message to delete.
        *
        * @throws HTTPException If the IronMQ service returns a status other than 200 OK.
        * @throws IOException If there is an error accessing the IronMQ server.
        */

        public void deleteMessage(Message msg)
        {
            deleteMessage(msg.Id);
        }

        /**
        * Pushes a message onto the queue.
        *
        * @param msg The body of the message to push.
        *
        * @throws HTTPException If the IronMQ service returns a status other than 200 OK.
        * @throws IOException If there is an error accessing the IronMQ server.
        */

        public void push(String msg)
        {
            push(msg, 0);
        }

        /**
        * Pushes a message onto the queue.
        *
        * @param msg The body of the message to push.
        * @param timeout The timeout of the message to push.
        *
        * @throws HTTPException If the IronMQ service returns a status other than 200 OK.
        * @throws IOException If there is an error accessing the IronMQ server.
        */

        public void push(String msg, long timeout)
        {
            push(new string[] { msg }, timeout);
        }

        /**
        * Pushes messages onto the queue.
        *
        * @param msgs The body of the messages to push.
        * @param timeout The timeout of the message to push.
        *
        * @throws HTTPException If the IronMQ service returns a status other than 200 OK.
        * @throws IOException If there is an error accessing the IronMQ server.
        */

        public void push(IEnumerable<string> msgs, long timeout = 0)
        {
            client.post("queues/" + name + "/messages",
                serializer.Serialize(new QueueMessages()
                {
                    messages = msgs.Select(msg => new Message() { Body = msg, Timeout = timeout }).ToArray(),
                }));
        }
    }
}