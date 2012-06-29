using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using io.iron.ironmq;
using io.iron.ironmq.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iron_mq_testing
{
    /// <summary>
    /// Summary description for IronMQUnitTest
    /// </summary>
    [TestClass]
    public class IronMQUnitTest
    {
        private readonly Credentials _credentials;

        public IronMQUnitTest()
        {
            _credentials = CredentialsRepository.LoadFrom("ironmq.credentials.txt");
            Assert.IsFalse(string.IsNullOrWhiteSpace(_credentials.ProjectId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(_credentials.Token));
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            var c = new Client(_credentials);
            var q = c.Queue("test-queue");
            ClearQueue(q);

            var body = "Hello, IronMQ!";
            q.Enqueue(body);

            var msg = q.Dequeue();
            Assert.IsTrue(string.Compare(body, msg.Body) == 0);
            q.Delete(msg);
        }

        private void ClearQueue(Queue q)
        {
            try
            {
                q.Clear();
            }
            catch { 
                //TODO: This is here because of a bug in the Endpoint where clearing an empty queue results in a 500 internal server error.
            }
        }

        [TestMethod]
        public void BasicTests()
        {
            var c = new Client(_credentials);
            var q = c.Queue("test-queue");

            ClearQueue(q);
            // clear_queue
            q.Enqueue("hello world!");
            var msg = q.Dequeue();
            Assert.IsNotNull(msg.Id);
            Assert.IsNotNull(msg.Body);

            q.Delete(msg.Id);
            msg = q.Dequeue();
            Assert.IsNull(msg);
            
            q.Enqueue("hello world 2!");

            msg = q.Dequeue();
            Assert.IsNotNull(msg);


            q.Delete(msg.Id);


            msg = q.Dequeue();
            Assert.IsNull(msg);
        }

        /// <summary>
        ///A test for push
        ///</summary>
        [TestMethod()]
        public void BulkPushTest()
        {
            var c = new Client(_credentials);
            var q = c.Queue("test-queue");

            ClearQueue(q);
            var messages = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
            const long timeout = 0; 
            q.Enqueue(messages, timeout);

            for (var i = 0; i < 10; i++)
            {
                var msg = q.Dequeue();
                Assert.IsNotNull(msg);
            }
            // Assumption is that if we queued up 1000 and we got back 1000 then it worked fine.
            // Note: this does not verify we got back the same messages
         
        }
        [TestMethod]
        public void BulkGetTest()
        {
            var c = new Client(_credentials);
            var q = c.Queue("test-queue");
            ClearQueue(q);

            var messages = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
            const long timeout = 0;
            q.Enqueue(messages, timeout);

            var actual = q.Dequeue(100);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 1);
        }
        /// <summary>
        /// Test for clearing a queue.
        /// </summary>
        [TestMethod]
        public void ClearQueueTest()
        {
            var c = new Client(_credentials);
            var q = c.Queue("test-queue");
            ClearQueue(q);

            const string messageBody = "This is a test of the emergency broadcasting system... Please stand by...";
            q.Enqueue(messageBody);
            q.Clear();

            var msg = q.Dequeue();
            Assert.IsNull(msg);
        }

        /// <summary>
        /// Test For Clearing an empty queue
        /// </summary>
        [TestMethod]
        public void ClearEmptyQueueTest()
        {
            var c = new Client(_credentials);
            var q = c.Queue("test-queue");
            ClearQueue(q); 
            // At this point the queue should be empty
            q.Clear();
        }
    }
}