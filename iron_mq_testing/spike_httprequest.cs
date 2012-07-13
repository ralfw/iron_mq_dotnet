using System;
using NUnit.Framework;
using System.Net;
using System.IO;
using System.Text;
using io.iron.ironmq;

namespace iron_mq_testing
{
	[TestFixture]
	public class spike_httprequest
	{
		[Test]
		public void Run ()
		{
			var sut = new Client(CredentialsRepository.LoadFrom("ironmq.credentials.txt"));
			Console.WriteLine("1");

			var q = sut.Queue("monotest");
			Console.WriteLine("2");

			q.Enqueue("test");
			Console.WriteLine("3");
		}
	}
}

