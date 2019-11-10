using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace medByteApi.Services
{
    public class MailService : IMailSender
    {
        public async Task SendEmailAsync(string Title, string Email, string Message)
        {
            try
            {

                var credentials = new NetworkCredential("medbytenoreply@gmail.com", "Asd456asd!");
                
                var mail = new MailMessage()
                {
                    From = new MailAddress("medbytenoreply@gmail.com"),
                    Subject = Title,
                    Body = Message
                };
                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress(Email));
                mail.CC.Add(new MailAddress(credentials.UserName));
                // Smtp client
                using (var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                })
                {
                    await client.SendMailAsync(mail);
                    return;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
