// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using RabbitMQBasicCommunication.Publisher;
using System.Text;

var factory = new ConnectionFactory()
{
    Uri = new Uri("amqps://vxmxyzmj:yBx86MiH5XnT5vXmEgVF2InBkDkJp6P2@sparrow.rmq.cloudamqp.com/vxmxyzmj")
};

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("logs-direct", durable: true,type: ExchangeType.Direct);

Enum.GetNames(typeof(LogType)).ToList().ForEach(x =>
{
    var queueName = $"direct-queue-{x}";
    channel.QueueDeclare(queueName, true, false, false);

    var routeKey = $"route-{x}";

    channel.QueueBind(queueName, "logs-direct",routeKey,null);
});

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    LogType logtype = (LogType)new Random().Next(1, 5);

    string message = $"log-type : {logtype}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    var routeKey = $"route-{logtype}";

    channel.BasicPublish("logs-direct", routeKey, null, messageBody);

    Console.WriteLine($"Log sent successfully ! {message}");
});

Console.ReadLine();