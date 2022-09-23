


using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;
using Spire.Doc;
using RabbitMQBasicCommunication.WordToPdf.Consumer;

bool result = false;
var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://vxmxyzmj:yBx86MiH5XnT5vXmEgVF2InBkDkJp6P2@sparrow.rmq.cloudamqp.com/vxmxyzmj");

using (var connection = factory.CreateConnection())
{
    using (var channel = connection.CreateModel())
    {
        var exchangeName = "convert-exchange";
        var queueName = "FileQueue";

        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);

        channel.QueueBind(queueName, exchangeName, "WordToPdf");

        channel.BasicQos(0, 1, false);

        var consumer = new EventingBasicConsumer(channel);


        channel.BasicConsume(queueName, false, consumer);

        consumer.Received += (model, ea) =>
        {
            try
            {
                Console.WriteLine("Get message and processing...");

                Document document = new Document();

                string deserializeString = Encoding.UTF8.GetString(ea.Body.ToArray());

                WordToPdfMessage wordToPdfMessage = JsonSerializer.Deserialize<WordToPdfMessage>(deserializeString);

                document.LoadFromStream(new MemoryStream(wordToPdfMessage.WordBytpe), FileFormat.Docx2013);

                using MemoryStream ms = new MemoryStream();
                document.SaveToStream(ms, FileFormat.PDF);

                result = Helper.EmailSend(wordToPdfMessage.Email, ms, wordToPdfMessage.FileName);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Error : "+ex.Message);
            }


            if (result)
            {
                Console.WriteLine("Mail send successfuly !");
                channel.BasicAck(ea.DeliveryTag, false);
            }
        };

        Console.WriteLine("Process completed!");
        Console.ReadLine();

    }
}
