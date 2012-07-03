IronMQ .NET Client
----------------

Getting Started
===============

[Clone the IronMQ Project](https://github.com/ralfw/iron_mq_dotnet). 

Check if in packages there is Newtonsoft´s Json library.
If not, download it using NuGet.

Compile the solution in Visual Studio. The assemblies to deploy will appear in a top level bin directory.


The Basics
==========
**Initialize** a client and get a queue object:

    Client client = new Client("my project", "my token");	// defualt Host and Port
    Queue queue = client.Queue("my_queue");

**Push** a message on the queue:

    queue.Enqueue("Hello, world!");

**Pop** a message off the queue:

    Message msg = queue.Dequeue();

When you pop/get a message from the queue, it will *not* be deleted. It will
eventually go back onto the queue after a timeout if you don't delete it. (The
default timeout is 60 seconds)

**Delete** a message from the queue:

    queue.Delete(msg);


Choosing Cloud
==============
**Initialize** a client and get a queue object (Amazon):

    Client client = new Client("my project", "my token", "mq-aws-us-east-1.iron.io");	// Amazon (default)

**Initialize** a client and get a queue object (Rackspace):
  
    Client client = new Client("my project", "my token", "mq-rackspace-dfw.iron.io");	// Rackspace


