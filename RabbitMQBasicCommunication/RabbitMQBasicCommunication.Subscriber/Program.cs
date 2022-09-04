// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory()
{
    Uri = new Uri("amqps://vxmxyzmj:yBx86MiH5XnT5vXmEgVF2InBkDkJp6P2@sparrow.rmq.cloudamqp.com/vxmxyzmj")
};

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

var queueName = "hello-queue";

//channel.QueueDeclare(queueName, true, false, false); => already created Publisher Library.  Than it is not necessary !

var consumer = new EventingBasicConsumer(channel);

//autoAck : set false when this message read it after automatically delete. set true after delete yourself 
channel.BasicConsume(queueName, autoAck: true,consumer);

consumer.Received += (object? sender, BasicDeliverEventArgs e) => {
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    Console.WriteLine($"Recived message : {message}");
};

Console.ReadLine();