using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class PowerPack
    {
        [Key]
        public int PowId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? TimeFrom { get; set; }
        public string? TimeTo { get; set; }
        public string? DataSource { get; set; }
    }
}