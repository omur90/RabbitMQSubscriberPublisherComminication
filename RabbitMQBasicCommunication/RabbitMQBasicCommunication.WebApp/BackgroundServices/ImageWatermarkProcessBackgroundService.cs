
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQBasicCommunication.WebApp.RabbitMQCommon;
using RabbitMQBasicCommunication.WebApp.Services;
using System.Drawing;
using System.Text;

namespace RabbitMQBasicCommunication.WebApp.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService rabbitMQClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> logger;
        private IModel channel;


        public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
        {
            this.rabbitMQClientService = rabbitMQClientService;
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            channel = rabbitMQClientService.Connect();

            channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var productImageCreatedEvent = System.Text.Json.JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                if (productImageCreatedEvent == null)
                {
                    throw new Exception("productImageCreatedEvent can not be null !");
                }

                if (string.IsNullOrWhiteSpace(productImageCreatedEvent.ImagePath))
                {
                    throw new Exception("ImagePath can not be null !");
                }

                var drawingText = "www.omurkurt.com.tr";

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", productImageCreatedEvent.ImagePath);

                using var img = Image.FromFile(path);

                using var graphic = Graphics.FromImage(img);

                var font = new Font(FontFamily.GenericMonospace, 38, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphic.MeasureString(drawingText, font);

                var color = Color.FromArgb(128, 255, 255);

                var brush = new SolidBrush(color);

                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height));

                graphic.DrawString(drawingText, font, brush, position);

                img.Save("wwwroot/images/watermarks/" + productImageCreatedEvent.ImagePath);

                img.Dispose();

                graphic.Dispose();

                channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                logger.LogInformation("Failed : " + ex.Message);
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
