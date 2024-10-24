using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class AppAccount
    {
        [Key]
        public int Aid { get; set; }
        public int? Uid { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? Lastname { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? ContractNo { get; set; }
        public string? Organization { get; set; }
        public string? Position { get; set; }
        public string? AuthorityLv { get; set; }
    }
}
