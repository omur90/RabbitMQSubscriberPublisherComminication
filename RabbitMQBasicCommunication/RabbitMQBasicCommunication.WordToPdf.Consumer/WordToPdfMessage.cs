
namespace RabbitMQBasicCommunication.WordToPdf.Consumer
{
    public class WordToPdfMessage
    {
        public byte[] WordBytpe { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
    }
}
