﻿using RabbitMQ.Client;

namespace RabbitMQBasicCommunication.ExcelApp.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;

        private IConnection _connection;
        private IModel _channel;
        private ILogger<RabbitMQClientService> _logger;
        public static string ExchangeName = "ExcelDirectExchange";
        public static string RoutingExcel = "excel-route-file";
        public static string QueueName = "queue-excel-file";

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct, true, false, null);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(QueueName, ExchangeName, RoutingExcel, null);

            _logger.LogInformation("connected with RabbitMQ successfully !");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Dispose();
            _connection?.Close();

            _logger.LogInformation("RabbitMQ connection broken !");

        }
    }
}
