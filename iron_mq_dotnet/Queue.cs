using System;
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
            client.post("queues/" + name + "/messages", serializer.Serialize(new QueueMessages() { messages = new Message[] { new Message() { Body = msg, Timeout = timeout } } }));
        }
    }
}
