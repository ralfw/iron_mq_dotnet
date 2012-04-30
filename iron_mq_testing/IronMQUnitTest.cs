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
		private string _projectId = null;
		private string _token = null;

		public IronMQUnitTest()
		{
			_projectId = ConfigurationManager.AppSettings["IRONIO_PROJECT_ID"];
			_token = ConfigurationManager.AppSettings["IRONIO_TOKEN"];
			Assert.IsFalse(string.IsNullOrWhiteSpace(_projectId));
			Assert.IsFalse(string.IsNullOrWhiteSpace(_token));
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
			Message msg = null;
			while ((msg = q.get()) != null)
			{
				q.deleteMessage(msg.Id);
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
			//queue = @client.queues.get(:name=>@client.queue_name)
			//total_messages = queue.total_messages
			//res = @client.messages.post("hello world!")
			//p res
			//assert res["id"]
			//assert res.id
			//assert res.msg

			//queue = @client.queues.get(:name=>@client.queue_name)
			//assert queue.size == 1
			//assert queue.total_messages == (total_messages+1)

			// msg = q.get();
			//   Assert.IsNotNull( msg.Id );
			//res = @client.messages.get()
			//p res
			//assert res["id"]
			//assert res.id

			q.deleteMessage(msg.Id);
			msg = q.get();
			Assert.IsNull(msg);
			//res = @client.messages.delete(res["id"])
			//p res
			//puts "shouldn't be any more"
			//res = @client.messages.get()
			//p res
			//assert res.nil?

			//queue = @client.queues.get(:name=>@client.queue_name)
			//assert queue.size == 0

			q.push("hello world 2!");
			//res = @client.messages.post("hello world 2!")
			//p res

			msg = q.get();
			Assert.IsNotNull(msg);
			//msg = @client.messages.get()
			//p msg
			//assert msg

			q.deleteMessage(msg.Id);
			//res = msg.delete
			//p res

			msg = q.get();
			Assert.IsNull(msg);
			//puts "shouldn't be any more"
			//res = @client.messages.get()
			//p res
			//assert res.nil?
		}

		/// <summary>
		///A test for push
		///</summary>
		[TestMethod()]
		public void bulkPushTest()
		{
			Client c = new Client(_projectId, _token);
			Queue q = c.queue("test-queue");

			ClearQueue(q);
			var messages = Enumerable.Range(0, 1000).Select(i => i.ToString()).ToArray();
			long timeout = 0; // TODO: Initialize to an appropriate value
			q.push(messages, timeout);
			var actual = new List<Message>(1000);
			for (int i = 0; i < 1000; i++)
			{
				var msg = q.get();
				Assert.IsNotNull(msg);
				actual.Add(msg);
			}
			// Assumption is that if we queued up 1000 and we got back 1000 then it worked fine.
			// Note: this does not verify we got back the same messages
			foreach (var msg in actual)
				q.deleteMessage(msg);
		}
	}
}