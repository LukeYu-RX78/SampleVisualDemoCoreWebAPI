using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Breakdowns
    {
        [Key]
        public int Bid { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? Type { get; set; }
        public string? Hours { get; set; }
        public string? DataSource { get; set; }
    }
}
