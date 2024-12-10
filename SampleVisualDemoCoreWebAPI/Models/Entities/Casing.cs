using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Casing
    {
        [Key]
        public int CasId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? HoleID { get; set; }
        public string? CasingType { get; set; }
        public string? CasingSize { get; set; }
        public string? DepthFrom { get; set; }
        public string? DepthTo { get; set; }
        public string? DataSource { get; set; }
    }
}
