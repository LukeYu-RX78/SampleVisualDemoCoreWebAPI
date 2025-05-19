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
        public DbSet<AppAccount> AppAccounts { get; set; }
        public DbSet<Plod> Plods { get; set; }
    }
}