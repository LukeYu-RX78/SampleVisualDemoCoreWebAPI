using System.Threading.Tasks;
using SampleVisualDemoCoreWebAPI.Utils;

namespace SampleVisualDemoCoreWebAPI.Infrastructure
{
    public class MailKitEmailService : IEmailService
    {
        private const string TemplatePath = "Templates/RejectionEmailTemplate.html";

        public string GetFormattedRejectionEmail(Dictionary<string, string> data)
        {
            return EmailTemplateLoader.LoadAndFormat(TemplatePath, data);
        }

        public async Task SendEmailAsync(string to, string subject, string bodyHtml)
        {
            // TODO: Replace with MailKit SMTP logic
            Console.WriteLine($"Email sent to {to}:\nSubject: {subject}\nBody: {bodyHtml}");
            await Task.CompletedTask;
        }
    }

}
