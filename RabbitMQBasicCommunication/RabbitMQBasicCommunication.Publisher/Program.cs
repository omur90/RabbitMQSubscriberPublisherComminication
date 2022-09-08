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

channel.ExchangeDeclare("header-exchange-example", durable: true,type: ExchangeType.Headers);

Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape2", "a4");
    
var properties = channel.CreateBasicProperties();
properties.Headers = headers;

channel.BasicPublish("header-exchange-example", string.Empty, properties,Encoding.UTF8.GetBytes("header message example !"));

Console.WriteLine("Header Topic example runnig !");

Console.ReadLine();