using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class RigService
    {
        [Key]
        public int RsId { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? ServiceType { get; set; }
        public string? Daily { get; set; }
        public string? Weekly { get; set; }
        public string? ThousandHour { get; set; }
        public string? DataSource { get; set; }
    }
}