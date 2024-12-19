using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Grouting
    {
        [Key]
        public int Gid { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? HoleID { get; set; }
        public string? DrillSite { get; set; }
        public string? BagsUsed { get; set; }
        public string? Metres { get; set; }
        public string? Volume { get; set; }
        public string? DataSource { get; set; }
    }
}