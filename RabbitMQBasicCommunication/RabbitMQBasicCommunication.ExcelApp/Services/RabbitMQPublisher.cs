using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace RabbitMQBasicCommunication.ExcelApp.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            this.rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(CreateExcelMessage createExcelMessage)
        {
            var channel = rabbitMQClientService.Connect();

            var bodyString = JsonSerializer.Serialize(createExcelMessage);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();

            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingExcel, basicProperties: properties, body: bodyByte);
        }
    }
}
