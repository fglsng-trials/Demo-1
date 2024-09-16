# Demo-1
This is a simple demo to show an understanding of creating a couple of connected microservices.


## Description
The demo is based on an assignment from a job interview. The assignment was to create two applications, a producer and a consumer, that communicate through a message broker, together with some business logic.

## Assignment / Requirements

* Make 2 applications (.NET) . They need to use a message broker (Azure eventhub , Rabbitmq, Kafka or other).
1. Producer app. Produces a message with a timestamp and a counter (int) in a loop with a delay.
2. Consumer app with the following business logic:
	1. Take message from queue. If the message timestamp is over 1 minute old - then discard it
	2. If the message is under 1 minute old and the second on the timestamp is an even number, then put the message in a database (azure db, postgres, mongo, other)
	3. If the message is under 1 minute old and the second on the timestamp is an odd number, then put the message back in the queue with a counter increment of +1
	4. Ensure the business logic is covered by unit tests.

* Bonus assignment and topics: Containers, docker-compose/kubernetes, SOLID principles, persistence of message queue and DB when restarting containers, clustering, logging, observability, async/await, dynamic configuration of values or other modifications of your choice.

## Solution
The solution is based on the requirements above. Both applications are based on .NET 8, "Worker Service" project template. 
The producer app is generating a message with a timestamp and a counter in a loop with a 1 second delay. The message is sent to a RabbitMQ queue.
The consumer app is taking the message from the queue and processing it according to the business logic. The message is then either stored in a MSSQL database, sent back to the queue with an incremented counter or discarded if the timestamp is over 1 minute old.
In general the solution is created as simple as possible, to show the understanding of the requirements.

The solution is using the following technologies:
* .NET 8
* RabbitMQ
* Entity Framework Core
* MSTest
* Redis
* Logging
* Docker
* Docker Compose
* MSSQL
* Dependecy Injection
* SOLID principles
* Asynchronous programming
* Configuration
