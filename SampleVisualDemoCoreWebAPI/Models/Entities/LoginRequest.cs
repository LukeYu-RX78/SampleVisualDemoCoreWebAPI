namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class LoginRequest
    {
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
    }
}