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


var consumer = new EventingBasicConsumer(channel);

var queueName = channel.QueueDeclare().QueueName;


Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");
//headers.Add("x-match", "all"); // all conditional must match
headers.Add("x-match", "any"); // at least one conditional must match

channel.QueueBind(queueName, "header-exchange-example", string.Empty, headers);

channel.BasicConsume(queueName, autoAck: false, consumer);

Console.WriteLine("Channel Listining...");

consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    System.Threading.Thread.Sleep(1000);

    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    
    Console.WriteLine($"Recived message : {message}");

    channel.BasicAck(e.DeliveryTag, multiple: false);
};

Console.ReadLine();