using System.ComponentModel.DataAnnotations;

namespace dors_backend.Models.Entities
{
    public class DrillReport
    {
        [Key]
        public int rid { get; set; }
        public required string refid { get; set; }
        public string uid { get; set; }
        public string contract_no { get; set; }
        public string? client {  get; set; }
        public string rigno {  get; set; }
        public string department { get; set; }
        public string dr_date { get; set; }
        public string dr_shift { get; set; }
        public string dr_day { get; set; }
        public string dr_datetype { get; set; }
        public string mach_hrs_from { get; set; }
        public string mach_hrs_to { get; set; }
        public string dr_location { get; set; }
        public string? comments { get; set; }
        public string report_state { get; set; }
    }
}
