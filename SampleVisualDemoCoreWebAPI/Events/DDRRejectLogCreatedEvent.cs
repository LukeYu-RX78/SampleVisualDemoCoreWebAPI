namespace SampleVisualDemoCoreWebAPI.Events
{
    public class DDRRejectLogCreatedEvent : IEvent
    {
        public int Pid { get; }
        public string Message { get; }
        public string ReviewerName { get; }
        public string ReviewerEmail { get; }
        public string UserFirstName { get; }
        public string UserEmail { get; }
        public string ContractNo { get; }
        public string RigNo { get; }
        public string PlodDate { get; }
        public string PlodShift { get; }
        public string RejectionDateTime { get; }

        public DDRRejectLogCreatedEvent(
            int pid,
            string message,
            string reviewerName,
            string reviewerEmail,
            string userFirstName,
            string userEmail,
            string contractNo,
            string rigNo,
            string plodDate,
            string plodShift,
            string rejectionDateTime)
        {
            Pid = pid;
            Message = message;
            ReviewerName = reviewerName;
            ReviewerEmail = reviewerEmail;
            UserFirstName = userFirstName;
            UserEmail = userEmail;
            ContractNo = contractNo;
            RigNo = rigNo;
            PlodDate = plodDate;
            PlodShift = plodShift;
            RejectionDateTime = rejectionDateTime;
        }
    }
}