// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    Uri = new Uri("amqps://vxmxyzmj:yBx86MiH5XnT5vXmEgVF2InBkDkJp6P2@sparrow.rmq.cloudamqp.com/vxmxyzmj")
};

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

var queueName = "hello-queue";

// durable = false save message in memory, set true save physically in rabbitMQ Server
//exclusive = set true acces only this channel, set false access with any subscriber.
//autoDelete = set true when all subscriber disconnect then message delete. Best practice says set false.
channel.QueueDeclare(queueName, true, false, false);

// message is a byte array for RabbitMQ.
string message = "hello word !";

var messageBody = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(string.Empty, queueName, null, messageBody);

Console.WriteLine("Message sent successfully !");

Console.ReadLine();