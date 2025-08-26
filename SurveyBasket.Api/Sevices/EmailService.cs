
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Sevices
{
    public class EmailService(IOptions<MailSettings> options) : IEmailService
    {
        private readonly MailSettings options = options.Value;

        public async Task SendEmailAsync(string email, string htmlMessage)
        {
            var message = new MimeKit.MimeMessage()
            {
                Sender = MailboxAddress.Parse(options.Mail),
                Subject = "Survey Basket - Confirm email"
            };
            message.To.Add(MailboxAddress.Parse(email));
            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            message.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(options.Host, options.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(options.Mail, options.Password);
            var result = await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true); 
            
        }
    }
}
