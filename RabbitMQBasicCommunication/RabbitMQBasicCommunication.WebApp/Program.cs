using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQBasicCommunication.WebApp.BackgroundServices;
using RabbitMQBasicCommunication.WebApp.Models;
using RabbitMQBasicCommunication.WebApp.RabbitMQCommon;
using RabbitMQBasicCommunication.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DispatchConsumersAsync => allow use async method
builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQConnection")), DispatchConsumersAsync = true });

builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();

builder.Services.AddHostedService<ImageWatermarkProcessBackgroundService>();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseInMemoryDatabase(databaseName: "productDb");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
