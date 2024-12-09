using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Drilling
    {
        [Key]
        public int Did { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? HoleID { get; set; } 
        public string? Angle { get; set; }
        public string? DrillType { get; set; }
        public string? Size { get; set; }
        public string? DepthFrom { get; set; }
        public string? DepthTo { get; set; }
        public string? TotalMetres { get; set; }
        public string? Barrel { get; set; }
        public string? RecoveredMetres { get; set; }
        public string? DCIMetres { get; set; }
        public string? NonChargeableMetres { get; set; }
        public string? DataSource { get; set; }
    }
}
