using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

        services.AddDbContext<WorkOrderDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });

        services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(configuration.GetConnectionString("RabbitMQConnection")), DispatchConsumersAsync = true });
        services.AddSingleton<RabbitMQClientService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
