namespace dors_backend.Models.Entities
{
    public class StagingPlod
    {
        public string? PlodDate { get; set; }
        public string? PlodShift { get; set; }
        public string? ContractNo { get; set; }
        public string? RigNo { get; set; }
        public string? Department { get; set; }
        public string? Client { get; set; }
        public string? DayType { get; set; }
        public string? Location { get; set; }
        public string? Comments { get; set; }
        public string? MachineHoursFrom { get; set; }
        public string? MachineHoursTo { get; set; }
        public string? TotalMetres { get; set; }
        public string? DrillingHrs { get; set; }
        public string? MetresperDrillingHr { get; set; }
        public string? TotalActivityHrs { get; set; }
        public string? MetresperTotalHr { get; set; }
        public string? VersionNumber { get; set; }
        public string? DataSource { get; set; }
    }
}
