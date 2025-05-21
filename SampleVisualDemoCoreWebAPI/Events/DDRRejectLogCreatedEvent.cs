namespace SampleVisualDemoCoreWebAPI.Events
{
    public class DDRRejectLogCreatedEvent : IEvent
    {
        public int Pid { get; }
        public string Message { get; }
        public string ReviewerEmail { get; }

        public DDRRejectLogCreatedEvent(int pid, string message, string reviewerEmail)
        {
            Pid = pid;
            Message = message;
            ReviewerEmail = reviewerEmail;
        }
    }
}