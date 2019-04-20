using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class SMTPEmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _options;

        public SMTPEmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Email, _options.Password),
                EnableSsl = _options.EnableSsl
            };

            using (client)
            {
                var mailMessage = new MailMessage(_options.Email, email, subject, message);

                await client.SendMailAsync(mailMessage); 
            }
        }
    }
}
