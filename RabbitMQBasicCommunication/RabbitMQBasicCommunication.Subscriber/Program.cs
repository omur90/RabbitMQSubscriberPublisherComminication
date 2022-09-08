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


var queueName = "direct-queue-Critical";


var consumer = new EventingBasicConsumer(channel);

channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

channel.BasicConsume(queueName, autoAck: false, consumer);

Console.WriteLine("Channel Listining...");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    System.Threading.Thread.Sleep(700);

    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    
    Console.WriteLine($"Recived message : {message}");

    File.AppendAllText("log-critical.txt", message + "\n");
   
    channel.BasicAck(e.DeliveryTag, multiple: false);
};

Console.ReadLine();