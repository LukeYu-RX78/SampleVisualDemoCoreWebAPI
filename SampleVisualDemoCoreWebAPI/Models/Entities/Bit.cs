using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Bit
    {
        [Key]
        public int BitId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? BitOrReamer { get; set; }
        public string? SerialNo { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }
        public string? HoleID { get; set; }
        public string? DepthFrom { get; set; }
        public string? DepthTo { get; set; }
        public string? TotalMetres { get; set; }
        public string? DataSource { get; set; }
    }
}