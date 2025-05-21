using SampleVisualDemoCoreWebAPI.Infrastructure;

namespace SampleVisualDemoCoreWebAPI.Events
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
            var subject = $"DDR #{@event.Pid} Rejected";
            var body = $"DDR with ID #{@event.Pid} was rejected.\n\nReason:\n{@event.Message}";
            await _emailService.SendEmailAsync(@event.ReviewerEmail, subject, body);
        }
    }
}
