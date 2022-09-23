using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQBasicCommunication.WordToPdf.Producer.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace RabbitMQBasicCommunication.WordToPdf.Producer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WordToPdfPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult WordToPdfPage(Models.WordToPdf wordToPdf)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri(_configuration.GetConnectionString("RabbitMQConnection"));

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    var exchangeName = "convert-exchange";
                    var queueName = "FileQueue";

                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);

                    channel.QueueDeclare(queueName, true, false, false, null);

                    channel.QueueBind(queueName, exchangeName, "WordToPdf");

                    var message = new WordToPdfMessage
                    {
                        Email = wordToPdf.Email,
                        FileName = Path.GetFileNameWithoutExtension(wordToPdf.File.FileName)
                    };

                    using MemoryStream ms = new();

                    wordToPdf.File.CopyTo(ms);
                    message.WordBytpe = ms.ToArray();

                    string serializeMessage = JsonSerializer.Serialize(message);

                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.Persistent = true;

                    channel.BasicPublish(exchangeName, "WordToPdf", basicProperties, Encoding.UTF8.GetBytes(serializeMessage));

                    ViewBag.result = "When the file convert, you will recieve details on your email !";
                }
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}