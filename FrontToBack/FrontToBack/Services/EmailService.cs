using FrontToBack.Interfeys;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace FrontToBack.Services
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string emailto, string body, string subject, bool ishtml = false)
        {
            SmtpClient smtp = new SmtpClient(_configuration["Email:Host"], Convert.ToInt32(_configuration["Email:Port"]));
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_configuration["Email:SignInEmail"], _configuration["Email:Password"]);

            MailAddress from = new MailAddress(_configuration["Email:SignInEmail"], "Pronia");
            MailAddress to = new MailAddress(emailto);

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = ishtml;

            await smtp.SendMailAsync(message);
        }

        
    }
}
