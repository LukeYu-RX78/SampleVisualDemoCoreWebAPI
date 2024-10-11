using System.ComponentModel.DataAnnotations;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class Plod
    {
        [Key]
        public int PlodID { get; set; }
        [Required]
        public string PlodDate { get; set; }
        [Required]
        public string PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? Department { get; set; }
        public string? DayType { get; set; }
        public string? NoCrew { get; set; }
        public string? Location { get; set; }
        public string? Comments { get; set; }
        public string? MachineHoursFrom { get; set; }
        public string? MachineHoursTo { get; set; }
        public bool? Checked { get; set; }
        public string? DataSource { get; set; }
        public string? LoadedDate { get; set; }
        public string? Loadedby { get; set; }
        public string? DatabaseComments { get; set; }
        public string? TimeStamp { get; set; }
        public string? ImporterVersion { get; set; }
        public string? ShiftRate { get; set; }
        public string? IMDEXFromID { get; set; }
        public string? TotalMetres { get; set; }
        public string? DrillingHrs { get; set; }
        public string? MetresperDrillingHr { get; set; }
        public string? TotalActivityHrs { get; set; }
        public string? MetresperTotalHr { get; set; }
        public string? Client { get; set; }
    }
}
