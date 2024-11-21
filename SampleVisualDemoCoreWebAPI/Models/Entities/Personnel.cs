using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Personnel
    {
        [Key]
        public int PerId { get; set; } 
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? Position { get; set; }
        public string? Name { get; set; }
        public string? Hours { get; set; }
        public string? TimeStart { get; set; }
        public string? TimeFinish { get; set; }
        public string? TravelDay { get; set; }
        public string? SickDay { get; set; }
        public string? DataSource { get; set; }
    }
}
