using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class SafetyData
    {
        [Key]
        public int Sid { get; set; }
        public int? Pid { get; set; }
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? JHA { get; set; }
        public string? SBO { get; set; }
        public string? Hazards { get; set; }
        public string? SWMSReviews { get; set; }
        public string? Observations { get; set; }
        public string? OperationsIncidents { get; set; }
        public string? Incidents { get; set; }
        public string? ReportableIncidents { get; set; }
        public string? LostTimeInjuries { get; set; }
        public string? MedicalTreatedInjuries { get; set; }
        public string? SiteInspections { get; set; }
        public string? Comments { get; set; }
        public string? DataSource { get; set; }
    }
}
