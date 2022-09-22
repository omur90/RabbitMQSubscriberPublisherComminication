using ClosedXML.Excel;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private RabbitMQClientService _rabbitmqClientService;
        private readonly IServiceProvider _serviceProvider;
        private IModel _channel;
        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitmqClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClientService.Connect();
            _channel.BasicQos(0, 1, false);


            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMQClientService.QueueName,false,consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(5000);

            var createExcelMessage = JsonSerializer.Deserialize<CreateExcelMessage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            if (createExcelMessage == null)
            {
                await Task.CompletedTask;
            }

            using var ms = new MemoryStream();

            var wb = new XLWorkbook();
            var ds = new DataSet();

            ds.Tables.Add(GetTable("WorkOrder"));

            wb.Worksheets.Add(ds);
            wb.SaveAs(ms);


            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();

            multipartFormDataContent.Add(new ByteArrayContent(ms.ToArray()), "file", Guid.NewGuid().ToString() + ".xlsx");

            var baseUrl = "https://localhost:7045/api/file";

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"{baseUrl}?fileId={createExcelMessage.FileId}", multipartFormDataContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Excel file (Id : {createExcelMessage.FileId}) was created successfuly !");
                _channel.BasicAck(@event.DeliveryTag, false);
            }
        }

        private DataTable GetTable(string tableName)
        {
            List<TblWorkOrder> workOrders;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WorkOrderDbContext>();

                if (dbContext == null)
                {
                    workOrders = new List<TblWorkOrder>();
                }

                workOrders = dbContext.TblWorkOrders.ToList();

            }

            DataTable dt = new() { TableName = tableName };

            dt.Columns.Add("Id", typeof(Guid));
            dt.Columns.Add("TaskId", typeof(string));
            dt.Columns.Add("EmployeeId", typeof(Guid));
            dt.Columns.Add("CreatedBy", typeof(string));

            workOrders.ForEach(x =>
            {
                dt.Rows.Add(x.Id, x.TaskId, x.EmployeeId, x.CreatedBy);
            });

            return dt;
        }
    }
}