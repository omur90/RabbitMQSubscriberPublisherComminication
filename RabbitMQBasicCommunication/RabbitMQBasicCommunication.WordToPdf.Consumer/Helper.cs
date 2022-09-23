using System;

using System.Net.Mail;
using System.Net.Mime;


namespace RabbitMQBasicCommunication.WordToPdf.Consumer
{
    public static class Helper
    {
        public static bool EmailSend(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                memoryStream.Position = 0;

                ContentType ct = new ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);

                Attachment attachment = new Attachment(memoryStream, ct);
                attachment.ContentDisposition.FileName = $"{fileName}.pdf";

                MailMessage mailMessage = new MailMessage();

                using SmtpClient smtpClient = new SmtpClient();

                mailMessage.From = new MailAddress("admin@xxx.net");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Pdf File Created !";
                mailMessage.Body = "Your pdf file is ready !";
                mailMessage.IsBodyHtml = true;
                mailMessage.Attachments.Add(attachment);

                smtpClient.Host = "mail.xxx.net";
                smtpClient.Port = 587;

                smtpClient.Credentials = new System.Net.NetworkCredential("admin@xxx.net", "xxx1234");

                smtpClient.Send(mailMessage);

                memoryStream.Dispose();
                memoryStream.Close();

                Console.WriteLine($"Mail send to : {email}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:"+ex.Message);
                return false;
            }
        }
    }
}
