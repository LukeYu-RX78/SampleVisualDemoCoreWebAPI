using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class DHSurvey
    {
        [Key]
        public int DhsId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? HoleID { get; set; }
        public string? Depth { get; set; }
        public string? ToolType { get; set; }
        public string? ToolNo { get; set; }
        public string? Dip { get; set; }
        public string? Azimuth { get; set; }
        public string? AziType { get; set; }
        public string? MagInt { get; set; }
        public string? MagDip { get; set; }
        public string? GravRoll { get; set; }
        public string? Temp { get; set; }
        public string? DataSource { get; set; }
    }
}