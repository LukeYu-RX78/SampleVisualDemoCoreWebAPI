using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Training
    {
        [Key]
        public int TrId { get; set; } 
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? Details { get; set; }
        public string? Instructor { get; set; }
        public string? Trainee { get; set; }
        public string? Time { get; set; }
        public string? DataSource { get; set; }
    }
}
