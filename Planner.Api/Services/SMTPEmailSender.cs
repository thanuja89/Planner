using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class SMTPEmailSender : IEmailSender
    {
        private readonly EmailSenderOptions _options;
        private readonly SmtpClient _client;

        public SMTPEmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;

            _client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Email, _options.Password),
                EnableSsl = true
            };
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage(_options.Email, email, subject, message);

            return _client.SendMailAsync(mailMessage);
        }
    }
}
