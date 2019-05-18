using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Planner.Api.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private SendGridEmailSenderOptions _options;

        public SendGridEmailSender(IOptions<SendGridEmailSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SendGridClient(_options.APIKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.FromEmail),
                Subject = subject,
                PlainTextContent = message
            };

            msg.AddTo(new EmailAddress(email));

            return client.SendEmailAsync(msg);
        }
    }
}
