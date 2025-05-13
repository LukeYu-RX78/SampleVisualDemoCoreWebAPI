using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    [Table("StagingTitelineAppDDRRejectLog")]
    public class DDRRejectLog
    {
        [Key]
        public int Lid { get; set; }
        public int? Pid { get; set; }
        public int? RejectedBy { get; set; }
        public int? RollBackTo { get; set; }
        public string? CreationDateTime { get; set; }
        public string? Message { get; set; }
    }
}
