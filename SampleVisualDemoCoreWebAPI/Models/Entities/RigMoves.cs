using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class RigMove
    {
        [Key]
        public int RmId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? HoleID { get; set; }
        public string? MoveType { get; set; }
        public string? DataSource { get; set; }
    }
}