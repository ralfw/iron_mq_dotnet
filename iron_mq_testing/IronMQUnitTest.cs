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
        private Credentials _credentials;

        public IronMQUnitTest()
        {
            _credentials = CredentialsRepository.LoadFrom("ironmq.credentials.txt");
            Assert.IsFalse(string.IsNullOrWhiteSpace(_credentials.ProjectId));
            Assert.IsFalse(string.IsNullOrWhiteSpace(_credentials.Token));
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            Client c = new Client(_projectId, _token);
            Queue q = c.queue("test-queue");
            ClearQueue(q);

            string body = "Hello, IronMQ!";
            q.push(body);

            Message msg = q.get();
            Assert.IsTrue(string.Compare(body, msg.Body) == 0);
            q.deleteMessage(msg);
        }

        private void ClearQueue(Queue q)
        {
            try
            {
                q.clear();
            }
            catch { 
                //TODO: This is here because of a bug in the Endpoint where clearing an empty queue results in a 500 internal server error.
            }
        }

        [TestMethod]
        public void BasicTests()
        {
            Client c = new Client(_projectId, _token);
            Queue q = c.queue("test-queue");

            ClearQueue(q);
            // clear_queue
            q.push("hello world!");
            Message msg = q.get();
            Assert.IsNotNull(msg.Id);
            Assert.IsNotNull(msg.Body);

            q.deleteMessage(msg.Id);
            msg = q.get();
            Assert.IsNull(msg);
            
            q.push("hello world 2!");

            msg = q.get();
            Assert.IsNotNull(msg);


            q.deleteMessage(msg.Id);


            msg = q.get();
            Assert.IsNull(msg);

        }

        /// <summary>
        ///A test for push
        ///</summary>
        [TestMethod()]
        public void BulkPushTest()
        {
            Client c = new Client(_projectId, _token);
            Queue q = c.queue("test-queue");

            ClearQueue(q);
            var messages = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
            long timeout = 0; 
            q.push(messages, timeout);

            for (int i = 0; i < 10; i++)
            {
                var msg = q.get();
                Assert.IsNotNull(msg);
            }
            // Assumption is that if we queued up 1000 and we got back 1000 then it worked fine.
            // Note: this does not verify we got back the same messages
         
        }
        [TestMethod]
        public void BulkGetTest()
        {
            Client c = new Client(_projectId, _token);
            Queue q = c.queue("test-queue");
            ClearQueue(q);

            var messages = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();
            long timeout = 0;
            q.push(messages, timeout);

            var actual = q.get(100);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 1);
        }
        /// <summary>
        /// Test for clearing a queue.
        /// </summary>
        [TestMethod]
        public void ClearQueueTest()
        {
            Client c = new Client(_projectId, _token);
            Queue q = c.queue("test-queue");
            ClearQueue(q);

            var messageBody = "This is a test of the emergency broadcasting system... Please stand by...";
            q.push(messageBody);
            q.clear();

            var msg = q.get();
            Assert.IsNull(msg);
        }

        /// <summary>
        /// Test For Clearing an empty queue
        /// </summary>
        [TestMethod]
        public void ClearEmptyQueueTest()
        {
            Client c = new Client(_projectId, _token);
            Queue q = c.queue("test-queue");
            ClearQueue(q); 
            // At this point the queue should be empty
            q.clear();
        }
    }
}