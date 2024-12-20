using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Fuel
    {
        [Key]
        public int Fid { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? Loads { get; set; }
        public string? Kilometres { get; set; }
        public string? Litres { get; set; }
        public string? Hours { get; set; }
        public string? Driver { get; set; }
        public string? DataSource { get; set; }
    }
}