namespace SampleVisualDemoCoreWebAPI.Infrastructure
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}