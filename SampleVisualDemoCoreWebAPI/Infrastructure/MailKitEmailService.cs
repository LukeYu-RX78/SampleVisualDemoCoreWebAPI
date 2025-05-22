using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace SampleVisualDemoCoreWebAPI.Infrastructure
{
    public class MailKitEmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public MailKitEmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string bodyHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DORS Notification", _emailSettings.Username));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = bodyHtml
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Console.WriteLine($"Email successfully sent to {to}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {to}: {ex.Message}");
                throw; // Optional: rethrow to handle upstream if needed
            }
        }
    }
}