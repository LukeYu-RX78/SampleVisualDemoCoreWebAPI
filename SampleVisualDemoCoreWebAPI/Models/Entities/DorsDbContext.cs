using Microsoft.EntityFrameworkCore;
using SampleVisualDemoCoreWebAPI.Models.Entities;

namespace SampleVisualDemoCoreWebAPI.Models.Entities
{
    public class DorsDbContext : DbContext
    {
        public DorsDbContext(DbContextOptions<DorsDbContext> options) : base(options)
        {
        }

        public DbSet<DDRRejectLog> DDRRejectLogs { get; set; }

        // Add other tables later as needed...
    }
}