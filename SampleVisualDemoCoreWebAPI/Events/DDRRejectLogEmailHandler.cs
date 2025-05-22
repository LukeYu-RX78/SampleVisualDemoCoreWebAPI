using SampleVisualDemoCoreWebAPI.Events;
using SampleVisualDemoCoreWebAPI.Infrastructure;
using SampleVisualDemoCoreWebAPI.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SampleVisualDemoCoreWebAPI.EventHandlers
{
    public class DDRRejectLogEmailHandler : IEventHandler<DDRRejectLogCreatedEvent>
    {
        private readonly IEmailService _emailService;

        public DDRRejectLogEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task HandleAsync(DDRRejectLogCreatedEvent @event)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "RejectionEmailTemplate.html");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException("Email template not found.", templatePath);
            }

            string template = await File.ReadAllTextAsync(templatePath);

            string body = template
                .Replace("{UserFirstName}", @event.UserFirstName)
                .Replace("{ContractNo}", @event.ContractNo)
                .Replace("{RigNo}", @event.RigNo)
                .Replace("{PlodDate}", @event.PlodDate)
                .Replace("{PlodShift}", @event.PlodShift)
                .Replace("{ReviewerName}", @event.ReviewerName)
                .Replace("{ReviewerEmail}", @event.ReviewerEmail)
                .Replace("{RejectionDateTime}", @event.RejectionDateTime)
                .Replace("{Message}", @event.Message);

            string subject = "Daily Drill Report Rejection Notice";

            await _emailService.SendEmailAsync(@event.UserEmail, subject, body);
        }
    }
}
