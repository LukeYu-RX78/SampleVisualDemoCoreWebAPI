using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Packing
    {
        [Key]
        public int PackId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? HoleID { get; set; }
        public string? Depth { get; set; }
        public string? Plugged { get; set; }
        public string? Comment { get; set; }
        public string? DataSource { get; set; }
    }
}
