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

channel.ExchangeDeclare("logs-topic", durable: true,type: ExchangeType.Topic);

Random rnd = new Random();
Enumerable.Range(1, 50).ToList().ForEach(x =>
{
 
    LogType logFirst = (LogType)new Random().Next(1, 5);
    LogType logSecond = (LogType)new Random().Next(1, 5);
    LogType logThird = (LogType)new Random().Next(1, 5);

    var routeKey = $"{logFirst}.{logSecond}.{logThird}";

    string message = $"log-type : {logFirst}-{logSecond}-{logThird}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("logs-topic", routeKey, null, messageBody);

    Console.WriteLine($"Log sent successfully ! {message}");
});

Console.ReadLine();