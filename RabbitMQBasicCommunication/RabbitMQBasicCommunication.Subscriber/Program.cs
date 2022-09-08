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


var randomQueueName = "log-database-save-queue"; //channel.QueueDeclare().QueueName;

//if not declare queue and when all subscriber down messages not recived.
//if declare queue not remove message on RabbitMq fanout exchange type.
//channel.QueueDeclare(randomQueueName, true, false, false); 

channel.QueueBind(randomQueueName, "logs-fanout", "", null);

//var queueName = "hello-queue";
//channel.QueueDeclare(queueName, true, false, false); => already created Publisher Library.  Than it is not necessary !



var consumer = new EventingBasicConsumer(channel);

//prefetchSize : 0 get any message 
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

//autoAck : set false when this message read it after automatically delete. set true after delete yourself 
channel.BasicConsume(randomQueueName, autoAck: false, consumer);

Console.WriteLine("Channel Listining...");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    System.Threading.Thread.Sleep(700);

    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    
    Console.WriteLine($"Recived message : {message}");

    //when message get successfuly than callBack to RabbitMQ 
    // multiple : only get a single message you should set false.
    channel.BasicAck(e.DeliveryTag, multiple: false);
};

Console.ReadLine();