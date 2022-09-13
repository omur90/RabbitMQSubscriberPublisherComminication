using RabbitMQ.Client;
using RabbitMQBasicCommunication.WebApp.Services;
using System.Text;
using System.Text.Json;

namespace RabbitMQBasicCommunication.WebApp.RabbitMQCommon
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            this.rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = rabbitMQClientService.Connect();

            var bodyString = JsonSerializer.Serialize(productImageCreatedEvent);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();

            properties.Persistent = true;

            channel.BasicPublish(exchange : RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingWatermark, basicProperties: properties, body: bodyByte);
        }
    }
}
