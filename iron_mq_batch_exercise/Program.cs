using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using io.iron.ironmq;

namespace iron_mq_batch_exercise
{
    class Program
    {
        static void Main(string[] args)
        {
            const string QUEUE_NAME = "batch_exercise";

            var cli = new Client(CredentialsRepository.LoadFrom("ironmq.credentials.txt"));
            var q = cli.Queue(QUEUE_NAME);

            q.Enqueue("hello " + DateTime.Now);
            var msg = q.Dequeue();

            Console.WriteLine(msg.Body);

            q.Delete(msg);
        }
    }
}
